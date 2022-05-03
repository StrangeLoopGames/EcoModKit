namespace Eco.WorldGenerator
{
    using Eco.Core.Utils;
    using Eco.Shared;
    using Eco.Shared.Math;
    using Eco.Shared.Utils;
    using Eco.Shared.Localization;
    using SharpNoise;
    using SharpNoise.Modules;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Threading.Tasks;
    using Color = System.Drawing.Color;
    using Eco.Core.Plugins.Interfaces;
    using Eco.Shared.IoC;
    using System.IO;
    
    public class ImageWorldGenerator : VoronoiWorldGenerator, IModKitPlugin
    {
        List<List<BiomePolygon>> lakes;
        
        public bool   SaveTerrainImage                       { get; private set; } = false;
        public bool   CustomWorldGeneratorEnabled            { get; set; }         = true;
        public string CustomWorldGeneratorBiomesImageName    { get; set; }         = "Biomes-import.png";
        public string CustomWorldGeneratorWaterImageName     { get; set; }         = "Water-import.png";
        public string CustomWorldGeneratorHeightmapImageName { get; set; }         = "Heightmap-import.png";
        
        public ImageWorldGenerator()
        {
            ServiceHolder<VoronoiWorldGenerator>.Obj = this;
        }
        
        public override void Generate(VoronoiWorldGeneratorConfig config)
        {
            this.rand = new System.Random(config.Seed);
            var sampler = new PoissonDiscSampler(this.WorldSize, this.WorldSize, config.PointRadius, this.rand);
            var sites = sampler.Samples().ToList();

            var numpoints = sites.Count;
            var offsets = new PointF[]
            {
                new PointF(-this.WorldSize, -this.WorldSize),
                new PointF(0,               -this.WorldSize),
                new PointF(this.WorldSize,  -this.WorldSize),

                new PointF(-this.WorldSize, 0),
                new PointF(this.WorldSize,  0),

                new PointF(-this.WorldSize, this.WorldSize),
                new PointF(0,               this.WorldSize),
                new PointF(this.WorldSize, this.WorldSize),
            };

            // duplicate the points to the 8 adjacent cells
            foreach (var offset in offsets)
            {
                for (int i = 0; i < numpoints; i++)
                {
                    float x = sites[i].X + offset.X;
                    float y = sites[i].Y + offset.Y;
                    sites.Add(new PointF(x, y));
                }
            }

            var ge = this.MakeVoronoiGraph(sites, this.WorldSize, this.WorldSize);

            // generate polygons from the voronoi points
            this.polygons = new BiomePolygon[numpoints];

            // prune outside edges that we don't care about
            var minBuffer = -config.PointRadius * 2f;
            var maxBuffer = this.WorldSize + (config.PointRadius * 2);
            ge.RemoveAll(e => e.x1 < minBuffer || e.x2 < minBuffer ||
                              e.x1 > maxBuffer || e.x2 > maxBuffer ||
                              e.y1 < minBuffer || e.y2 < minBuffer ||
                              e.y1 > maxBuffer || e.y2 > maxBuffer);

            var numWorkers = ProcessorUtils.GetAvailableProcessorCount() * 2;
            var jobsPerWorker = (int)Math.Ceiling((float)numpoints / numWorkers);

            //for (int i = 0; i < numpoints; i++)
            for (int workerIndex = 0; workerIndex < numWorkers; workerIndex++)
            //Parallel.For(0, numWorkers, workerIndex =>
            {
                var startIndex = workerIndex * jobsPerWorker;
                var endIndex = Math.Min(numpoints, startIndex + jobsPerWorker);
                for (var i = startIndex; i < endIndex; i++)
                {
                    // find all segments that reference this point
                    var points = new List<PointF>();
                    var adjacent = new HashSet<int>();
                    foreach (var edge in ge)
                    {
                        if (edge.site1 == i || edge.site2 == i)
                        {
                            if (edge.site1 != i)
                                adjacent.Add(edge.site1 % numpoints); // using modulus to get wrapped point positions
                            if (edge.site2 != i)
                                adjacent.Add(edge.site2 % numpoints); // using modulus to get wrapped point positions

                            var p1 = new PointF((float)edge.x1, (float)edge.y1);
                            var p2 = new PointF((float)edge.x2, (float)edge.y2);
                            // add the point if its new
                            bool tooClose = false;
                            foreach (var p in points)
                            {
                                float dx = p1.X - p.X;
                                float dy = p1.Y - p.Y;
                                if ((dx * dx) + (dy * dy) < .001f)
                                {
                                    tooClose = true;
                                    break;
                                }
                            }

                            if (!tooClose)
                                points.Add(p1);

                            // other point
                            tooClose = false;
                            foreach (var p in points)
                            {
                                var dx = p2.X - p.X;
                                var dy = p2.Y - p.Y;
                                if ((dx * dx) + (dy * dy) < .001f)
                                {
                                    tooClose = true;
                                    break;
                                }
                            }

                            if (!tooClose)
                                points.Add(p2);
                        }
                    }

                    // sort points
                    var pointCount = points.Count;
                    var center = new PointF(points.Sum(p => p.X) / pointCount, points.Sum(p => p.Y) / pointCount);

                    this.polygons[i] = new BiomePolygon()
                    {
                        Points = points.OrderBy(p => Math.Atan2(p.X - center.X, p.Y - center.Y)).Select(p =>
                        {
                            var dx = p.X - center.X;
                            var dy = p.Y - center.Y;
                            return new PointF(p.X + (dx * .01f), p.Y + (dy * .01f));
                        }).ToArray(),
                        Site = sites[i],
                        Adjacent = adjacent.ToArray(),
                        Center = center,
                        Index = i,
                    };
                }
            }
            //);

            // do biome stuff

            // CONFIGURATION ////////
            config.Initialize(this.rand);

            if (CustomWorldGeneratorEnabled)
            {
                bool generationSuccess = this.GenerateCustomWorld(config);

                if (generationSuccess)
                {
                    return;
                }
            }

            var landPercent       = config.LandPercent;
            var oceanPercent      = 1f - landPercent; // unused
            var islandPercent     = config.IslandWeight * landPercent; // percent of land that is tiny islands
            var continentPercent  = landPercent - islandPercent;

            // land
            var steppePercent     = config.SteppeWeight     * landPercent;
            var desertPercent     = config.DesertWeight     * landPercent;
            var highDesertPercent = config.HighDesertWeight * landPercent;
            var warmForestPercent = config.WarmForestWeight * landPercent;
            var coolForestPercent = config.CoolForestWeight * landPercent;
            var taigaPercent      = config.TaigaWeight      * landPercent;
            var tundraPercent     = config.TundraWeight     * landPercent;
            var icePercent        = config.IceWeight        * landPercent;
            var rainforestPercent = config.RainforestWeight * landPercent;
            var wetlandPercent    = config.WetlandWeight    * landPercent;
            // rest is coast/lakes/rivers

            // features percentages
            var numContinents       = config.NumContinents;
            var numSmallIslands     = config.NumSmallIslands;   // this is not guaranteed (could be higher) - small islands will be continually made to fit the land quota

            var modifier = config.ScaleModifier;
            var linearInverseModifier = config.InverseLinearScaleModifier;
            var numRainforests      = (int)Math.Round(config.NumRainforests * modifier);
            var numWarmForests      = (int)Math.Round(config.NumWarmForests * modifier);
            var numCoolForests      = (int)Math.Round(config.NumCoolForests * modifier);
            var numTaigas           = (int)Math.Round(config.NumTaigas * modifier);
            var numTundras          = (int)Math.Round(config.NumTundras * modifier);
            var numIces             = (int)Math.Round(config.NumIces * modifier);
            var numDeserts          = (int)Math.Round(config.NumDeserts * modifier);
            var numHighDeserts      = (int)Math.Round(config.NumHighDeserts * modifier);
            var numSteppes          = (int)Math.Round(config.NumSteppes * modifier);
            var numWetlands         = (int)Math.Round(config.NumWetlands * modifier);

            // BIOMES /////////////////////////////////////////////////////////

            // land prioritizer (continents)
            var landNoise = new Perlin() { Frequency = .5f * config.ScaleModifier, Quality = NoiseQuality.Best };
            var seamlessLand = new SeamlessNoise() { Source = landNoise };
            var terracedLand = new FlatTerraceModule() { Source = seamlessLand, NumTerraces = 4 };
            Func<BiomePolygon, float> landPrioritizer = p => (float)terracedLand.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);

            // island prioritizer
            var islandNoise = new Perlin() { Frequency = 1f * config.ScaleModifier, Quality = NoiseQuality.Best };
            var seamlessIsland = new SeamlessNoise() { Source = islandNoise };
            var terracedIsland = new FlatTerraceModule() { Source = seamlessIsland, NumTerraces = 4 };
            Func<BiomePolygon, float> islandPrioritizer = p => (float)terracedIsland.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);

            // biome prioritizer
            var biomeNoise = new Perlin() { Frequency = .5f * config.ScaleModifier, Quality = NoiseQuality.Best };
            var seamlessBiome = new SeamlessNoise() { Source = biomeNoise };
            var terracedBiome = new FlatTerraceModule() { Source = seamlessBiome, NumTerraces = 4 };
            Func<BiomePolygon, float> biomePrioritizer = p => (float)terracedBiome.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);

            // continents are the larger islands that spawn farther apart
            {
                var remainingContinentArea = continentPercent;
                var approximateContinentSize = remainingContinentArea / numContinents;
                bool perfectlyBalanced;
                do
                {
                    landNoise.Seed = this.rand.Next();

                    var landSize = Math.Min(remainingContinentArea, (((float)this.rand.NextDouble() * 1.5f) + .5f) * approximateContinentSize); // 50 - 200% of approx continent size
                    if (numContinents == 1)
                        landSize = remainingContinentArea; // special case 1-continent

                    var continentAvoidance = (int)Math.Round(config.ContinentAvoidRange.min + (this.rand.NextDouble() * config.ContinentAvoidRange.Diff));
                    var validPositions = new HashSet<BiomePolygon>(this.polygons.Where(p =>
                    {
                        if (p.Biome != Biome.DeepOcean)
                            return false;

                        var nearby = this.polygons.AdjacentPolygons(p.Index, continentAvoidance);
                        var nearbyLand = nearby.Count(a => this.polygons[a].Biome == Biome.Grassland);
                        return nearbyLand == 0;
                    }));
                    perfectlyBalanced = this.BalanceBiome(this.polygons, landSize, null, p => validPositions.Contains(p), landPrioritizer);
                    this.polygons.Where(p => p.Biome == null).ForEach(p => p.Biome = Biome.Grassland);

                    remainingContinentArea -= landSize;
                }
                while (perfectlyBalanced && remainingContinentArea > 0f);
            }

            // small islands
            {
                var remainingIslandArea = islandPercent;
                var approximateIslandSize = remainingIslandArea / numSmallIslands;
                bool perfectlyBalanced;
                do
                {
                    islandNoise.Seed = this.rand.Next();

                    var landSize = Math.Min(remainingIslandArea, (((float)this.rand.NextDouble() * 1.5f) + .5f) * approximateIslandSize); // 50 - 200% of approx island size
                    if (numSmallIslands == 1) landSize = remainingIslandArea; // special case 1-island

                    var islandAvoidance = (int)Math.Round(config.IslandAvoidRange.min + (this.rand.NextDouble() * config.IslandAvoidRange.Diff));
                    var validPositions = new HashSet<BiomePolygon>(this.polygons.Where(p =>
                    {
                        if (p.Biome != Biome.DeepOcean)
                            return false;

                        var nearby = this.polygons.AdjacentPolygons(p.Index, islandAvoidance);
                        var nearbyLand = nearby.Count(a => this.polygons[a].Biome == Biome.Grassland);
                        return nearbyLand == 0;
                    }));
                    perfectlyBalanced = this.BalanceBiome(this.polygons, landSize, null, p => validPositions.Contains(p), islandPrioritizer);
                    this.polygons.Where(p => p.Biome == null).ForEach(p => p.Biome = Biome.Grassland);

                    remainingIslandArea -= landSize;
                }
                while (perfectlyBalanced && remainingIslandArea > 0f);
            }

            // set coastline (land near ocean)
            for (var coastLineWidth = 0; coastLineWidth < config.CoastlineSize; coastLineWidth++)
            {
                var coastLine = this.polygons.Where(p => p.Biome == Biome.DeepOcean && p.Adjacent.Any(a => this.polygons[a].Biome != Biome.DeepOcean)).ToArray();
                foreach (var polygon in coastLine)
                    polygon.Biome = Biome.Coast;
            }

            // set shallow coastline (deep ocean near the coast) - do a few passes to get desired size
            for (var shallowOcean = 0; shallowOcean < config.ShallowOceanSize; shallowOcean++)
            {
                var ocean = this.polygons.Where(p => p.Biome == Biome.DeepOcean && p.Adjacent.Any(a => this.polygons[a].Biome != Biome.DeepOcean)).ToArray();
                foreach (var polygon in ocean)
                    polygon.Biome = Biome.Ocean;
            }

            // to prevent odd coastlines, set any coast now that isn't near water to grassland
            {
                var badCoastPolys = this.polygons.Where(p => p.Biome == Biome.Coast && this.polygons.AdjacentPolygons(p.Index, config.CoastlineSize).All(a => this.polygons[a].Biome.IsLand()));
                foreach (var badCoast in badCoastPolys) badCoast.Biome = Biome.Grassland;
            }

            // Cold Forest
            this.GenerateBiome(Biome.ColdForest, Biome.Grassland, biomePrioritizer, coolForestPercent, numCoolForests, biomeNoise);

            // Taiga
            Func<BiomePolygon, float> taigaPrioritizer = (BiomePolygon p) => (float)this.polygons.DistanceTo(p.Index, poly => poly.Biome == Biome.ColdForest || poly.Biome == Biome.Taiga);
            this.GenerateBiome(Biome.Taiga, Biome.ColdForest, taigaPrioritizer, taigaPercent, numTaigas, biomeNoise);

            // Tundra
            Func<BiomePolygon, float> tundraPrioritizer = (BiomePolygon p) => (float)this.polygons.DistanceTo(p.Index, poly => poly.Biome == Biome.Taiga || poly.Biome == Biome.Tundra);
            this.GenerateBiome(Biome.Tundra, Biome.Taiga, tundraPrioritizer, tundraPercent, numTundras, biomeNoise);

            // Ice
            Func<BiomePolygon, float> icePrioritizer = (p) => (float)this.polygons.DistanceTo(p.Index, poly => poly.Biome == Biome.Tundra || poly.Biome == Biome.Taiga || poly.Biome == Biome.Ice || poly.Biome == Biome.Coast);
            this.GenerateBiome(Biome.Ice, Biome.Tundra, icePrioritizer, icePercent, numIces, biomeNoise, true, false);

            // Warm Forest
            this.GenerateBiome(Biome.WarmForest, Biome.Grassland, biomePrioritizer, warmForestPercent, numWarmForests, biomeNoise);

            // Rainforest
            this.GenerateBiome(Biome.RainForest, Biome.Grassland, biomePrioritizer, rainforestPercent, numRainforests, biomeNoise);

            // Desert
            this.GenerateBiome(Biome.Desert, Biome.Grassland, biomePrioritizer, desertPercent, numDeserts, biomeNoise);

            // High Desert
            this.GenerateBiome(Biome.HighDesert, Biome.Desert, biomePrioritizer, highDesertPercent, numHighDeserts, biomeNoise, false, false);

            // Steppe
            this.GenerateBiome(Biome.Steppe, Biome.Grassland, biomePrioritizer, steppePercent, numSteppes, biomeNoise, false, false);

            // Wetland
            Func<BiomePolygon, float> wetlandPrioritizer =  p => (float)this.polygons.DistanceTo(p.Index, poly => poly.Biome == Biome.WarmForest || poly.Biome == Biome.Wetland);
            this.GenerateBiome(Biome.Wetland, Biome.WarmForest, wetlandPrioritizer, wetlandPercent, numWetlands, biomeNoise);

            /// CLEANUP ///////
            {
                // cleanup small bits of the map that might have too small of biomes
                var isolatedPolygons = this.polygons.Where(p => p.Adjacent.Count(a => this.polygons[a].Biome == p.Biome) < 2).ToArray();
                var result = new Dictionary<BiomePolygon, Biome>();
                foreach (var i in isolatedPolygons)
                {
                    var biomes = i.Adjacent.Select(p => this.polygons[p]);
                    var groups = biomes.GroupBy(p => p.Biome);
                    var mostCommon = groups.OrderByDescending(grp => grp.Count()).First().Key;

                    if (i.Biome != Biome.Coast && mostCommon == Biome.Coast)
                        continue; // special case, don't change things into coast if they aren't coast (keeps islands in tact)

                    result.Add(i, mostCommon);
                }

                foreach (var pair in result)
                    pair.Key.Biome = pair.Value;
            }

            /// ELEVATION, TEMPERATURE, MOISTURE //////////////////////////////////////////////////////////////
            // after biomes are assigned, do height, temperature, moisture

            // first set maximum height for each polygon based on distance to the ocean
            foreach (var p in this.polygons.Where(p => p.Biome.IsLand()))
            {
                float d = this.polygons.DistanceToOcean(p.Index);
                p.MaxElevation = Eco.Shared.Mathf.Pow(d * (1f / config.MaxElevationOceanDistance), config.ElevationPower).Clamp(0f, 1f);
            }

            // use simple noise to set initial heights, use to randomize around biome height range
            var elevationNoiseModule = new RidgedMulti() { Seed = this.rand.Next(), Frequency = 6f * config.InverseScaleModifier };
            var scaleBias = new ScaleBias() { Source0 = elevationNoiseModule, Scale = .5f, Bias = .5f };    // scale/bias to 0-1
            var elevationNoise = new SeamlessNoise() { Source = scaleBias };                                // make it tile

            var heightNoise = new Perlin() { Seed = this.rand.Next(), Frequency = 10f * config.InverseScaleModifier };  // [-1, 1]
            var seamlessHeightNoise = new SeamlessNoise() { Source = heightNoise };

            var moistureNoiseModule = new Perlin() { Seed = this.rand.Next(), Frequency = 5f * config.InverseScaleModifier };
            scaleBias = new ScaleBias() { Source0 = moistureNoiseModule, Scale = .5f, Bias = .5f };         // scale/bias to 0-1
            var moistureNoise = new SeamlessNoise() { Source = scaleBias };                                 // make it tile

            var temperatureNoiseModule = new Perlin() { Seed = this.rand.Next(), Frequency = 5f * config.InverseScaleModifier };
            scaleBias = new ScaleBias() { Source0 = temperatureNoiseModule, Scale = .5f, Bias = .5f };      // scale/bias to 0-1
            var temperatureNoise = new SeamlessNoise() { Source = scaleBias };                              // make it tile

            // randomize height values within the biome range, then blur a bit
            foreach (var p in this.polygons)
            {
                var elevationMod = (float)elevationNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize); // [0, 1]
                var heightMod = (float)seamlessHeightNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);
                var moistureMod = (float)moistureNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);
                var tempMod = (float)temperatureNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);

                var avgElevation = p.Biome.ElevationRange.Mid;
                var startElevation = avgElevation * elevationMod;
                var mod = heightMod * (p.Biome.ElevationRange.Diff * .5f); // multiply half the diff by -1, 1 to map to -diff, +diff

                var height = startElevation + mod;
                var temperature = p.Biome.TemperatureRange.min + (tempMod * p.Biome.TemperatureRange.Diff);
                var moisture = p.Biome.MoistureRange.min + (moistureMod * p.Biome.MoistureRange.Diff);

                p.Elevation = p.Biome.IsLand() ? height.Clamp(.05f, p.MaxElevation) : height.Clamp(-1f, -.05f);

                p.Temperature = temperature;
                p.Moisture = moisture;
            }

            // special case coastline - set them to average temp/moisture of cells nearby that aren't coast/ocean
            foreach (var p in this.polygons.Where(p => p.Biome == Biome.Coast))
            {
                var nearby = this.polygons.AdjacentPolygons(p.Index, config.CoastlineSize).Select(i => this.polygons[i]).Where(n => n.Biome != Biome.Coast && n.Biome.IsLand());
                if (nearby.Any())
                {
                    var avgTemp = nearby.Average(n => n.Temperature);
                    var avgMoisture = nearby.Average(n => n.Moisture);
                    p.Temperature = avgTemp;
                    p.Moisture = avgMoisture;
                }
            }

            // LAKES
            var numLakes = config.NumLakes;
            landNoise.Seed = this.rand.Next();
            var originalBiomes = this.polygons.ToDictionary(p => p, p => p.Biome);
            for (var i = 0; i < numLakes; i++)
            {
                biomeNoise.Seed = this.rand.Next();
                var lakeSize = (float)((config.LakeSizeRange.min * linearInverseModifier) + (this.rand.NextDouble() * config.LakeSizeRange.Diff * linearInverseModifier));

                // mark the lake as null biome initially
                this.BalanceBiome(this.polygons, lakeSize, null, p =>
                {
                    if (p.Biome.CanSpawnLake())
                    {
                        // don't spawn lake next to coast or another lake, etc (4 tile boundary)
                        var nearby = this.polygons.AdjacentPolygons(p.Index, 4);
                        return !nearby.Any(
                            a => this.polygons[a].HasLake ||
                                 this.polygons[a].Biome.IsOcean() ||
                                 this.polygons[a].Biome.IsDesert());
                    }
                    return false;
                }, biomePrioritizer);

                // clean up the lake slightly, so its a bit more 'filled in'
                var nearLakeTiles = this.polygons.Where(p => p.Biome != null && p.Adjacent.Count(a => this.polygons[a].Biome == null) >= 3);
                foreach (var nearLakePoly in nearLakeTiles)
                    nearLakePoly.Biome = null;

                // make this lake the same elevation.
                var nearLake = this.polygons.Where(p => p.Adjacent.Any(a => this.polygons[a].Biome == null) && p.Biome != null);
                if (!nearLake.Any())
                    continue;

                // use average height as the lake elevation
                // then make any nearby cells that aren't high enough, higher than the lake elevation
                var averageNearLake = nearLake.Average(p => p.Elevation);
                var lakeElevation = averageNearLake - .01f;
                foreach (var poly in nearLake.Where(p => p.Elevation <= lakeElevation))
                    poly.Elevation = lakeElevation + .01f;

                foreach (var lake in this.polygons.Where(p => p.Biome == null))
                {
                    lake.Elevation = lakeElevation;
                    lake.HasLake = true;
                    lake.Biome = originalBiomes[lake];
                }
            }

            // RIVERS ///////////////////////////////////////////////////////////
            var originalElevations = this.polygons.ToDictionary(p => p, p => p.Elevation);

            var numRiverAttempts = 512;
            var numRiversDesired = config.NumRivers;
            var allRivers = new List<List<BiomePolygon>>();

            var startPositions = this.polygons
                .Where(p => p.Biome.IsLand() && p.Biome != Biome.Ice)
                .OrderByDescending(p => this.polygons.DistanceToOcean(p.Index))
                .ThenByDescending(p => p.Elevation)
                .ToList();
            for (var r = 0; r < numRiverAttempts; r++)
            {
                if (startPositions.Count <= 0)
                    break;

                var start = startPositions[0];
                startPositions.RemoveAt(0);

                var river = new List<BiomePolygon>();
                var currentRiver = new HashSet<BiomePolygon>();
                river.Add(start);
                currentRiver.Add(start);
                start.HasRiver = true;

                // flow from start till we reach the ocean
                var current = start;
                var backTrack = 0;
                while (current.Biome != Biome.Ocean)
                {
                    // if last location was a lake, and we are no longer in a lake, be sure to mark the rest of the lake
                    // as part of this river, so we don't flow back into it later.
                    if (river.Count > 1)
                    {
                        var last = river[river.Count - 2];
                        if (last.HasLake && !current.HasLake)
                        {
                            // flood fill the entire lake
                            var floodFill = new Queue<BiomePolygon>();
                            var visited = new HashSet<BiomePolygon>();
                            floodFill.Enqueue(last);
                            visited.Add(last);
                            while (floodFill.Any())
                            {
                                var entry = floodFill.Dequeue();
                                foreach (var adj in entry.Adjacent.Select(a => this.polygons[a]).Where(p => p.HasLake))
                                {
                                    if (visited.Add(adj))
                                        floodFill.Enqueue(adj);
                                }
                            }

                            foreach (var cell in visited)
                            {
                                cell.HasRiver = true;
                                currentRiver.Add(cell);
                            }
                        }
                    }

                    // easy case - if next to another river or lake thats not us, the coast, or the ocean, just flow into it
                    var nearbyWater = current.Adjacent.Where(a =>
                        !currentRiver.Contains(this.polygons[a]) &&                                     // isn't part of our river
                        (this.polygons[a].HasRiver ||                                                   // and nearby cell is a river
                         this.polygons[a].HasLake ||                                                    // or is a lake
                        (this.polygons[a].Biome == Biome.Coast && !current.Biome.IsOcean()) ||          // or the coast (if not already in coast or ocean)
                        (this.polygons[a].Biome == Biome.Ocean) ||                                      // or the ocean
                        (this.polygons[a].Biome == Biome.DeepOcean))).ToArray();
                    if (!current.HasLake && nearbyWater.Length > 0)
                    {
                        current = this.polygons[nearbyWater[this.rand.Next(0, nearbyWater.Length)]];
                    }
                    else
                    {
                        // flow downwards
                        var riverAvoidance = config.RiverCellAvoidance;
                        var lower = current.Adjacent.Where(a =>
                                this.polygons[a].Biome != Biome.Ice &&                                                      // don't flow through ice cells
                                this.polygons[a].Elevation <= current.Elevation &&                                          // nearby cell with lower or equal elevation
                                !currentRiver.Contains(this.polygons[a]) &&                                                 // that doesn't already contain this river
                                this.polygons.AdjacentPolygons(this.polygons[a].Index, riverAvoidance).Count(ad => this.polygons[ad].HasRiver) <= riverAvoidance) // and only adjacent to one other river
                            .ToArray();                                                                                     // and isn't next to more rivers than the current one

                        if (lower.Length == 0)
                        {
                            // just get the closest in elevation nearby and lower it, then use that for the river
                            var available = current.Adjacent.Where(
                                a => this.polygons[a].Biome != Biome.Ice &&
                                     !currentRiver.Contains(this.polygons[a]) &&
                                     this.polygons.AdjacentPolygons(this.polygons[a].Index, riverAvoidance).Count(ad => this.polygons[ad].HasRiver) <= riverAvoidance).ToArray();

                            if (available.Length == 0)
                            {
                                for (var i = 0; i < backTrack; i++)
                                {
                                    // river seems to have painted itself into a corner... try going back
                                    river.RemoveAt(river.Count - 1);
                                    currentRiver.Remove(current);
                                    var old = current;

                                    old.HasRiver = false;
                                    if (!old.HasLake)
                                        old.Elevation = originalElevations[old];

                                    if (river.Count == 0)
                                        break; // bad river.

                                    current = river[river.Count - 1];
                                }

                                backTrack++;

                                if (river.Count == 0)
                                    break; // bad river.

                                continue;
                            }

                            var lowest = this.polygons[available.OrderBy(a => Math.Abs(current.Elevation - this.polygons[a].Elevation)).First()];
                            lowest.Elevation = current.Elevation;

                            current = lowest;
                        }
                        else
                        {
                            // pick the highest of the low areas, this tends to create much more meander-y rivers
                            var selection = this.polygons[lower.OrderByDescending(l => this.polygons[l].Elevation).First()];
                            current = selection;
                        }
                    }

                    river.Add(current);
                    currentRiver.Add(current);

                    if (current.HasRiver)
                        break; // done, met up with an existing river.

                    current.HasRiver = true;
                }

                if (river.Count != 0)
                    allRivers.Add(river);
            }

            // take the longest rivers
            var numTakenRivers = 0;
            this.rivers = new List<List<BiomePolygon>>();
            var orderedRivers = allRivers.OrderByDescending(river => river.Count).ToList();
            do
            {
                var river = orderedRivers[0];
                orderedRivers.RemoveAt(0);

                // last bit of cleanup - remove lake-lake segments of the river
                // segment river into segments if it encountered a lake, skipping the extra lake segments
                var segment = new List<BiomePolygon>();
                segment.Add(river[0]);
                for (var i = 1; i < river.Count; i++)
                {
                    segment.Add(river[i]);
                    if (river[i].HasLake)
                    {
                        this.rivers.Add(segment);
                        segment = new List<BiomePolygon>();
                        if (i < river.Count - 1)
                        {
                            while (i < (river.Count - 1) && river[i + 1].HasLake)
                                i++;

                            if (i < river.Count - 1)
                                segment.Add(river[i]);
                        }
                    }
                }
                if (segment.Count > 0) this.rivers.Add(segment);
                numTakenRivers++;
            }
            while (numTakenRivers < numRiversDesired && orderedRivers.Count > 0);

            // clean out all the unused rivers
            foreach (var poly in this.polygons) poly.HasRiver = false;
            foreach (var cell in this.rivers.SelectMany(x => x)) cell.HasRiver = true;

            // clean up elevation changes for anything that didn't end up a river
            foreach (var (polygon, elevation) in originalElevations.Where(pair => !pair.Key.HasRiver)) polygon.Elevation = elevation;

            // river cleanup - ensure the elevation changes are valid for each segment of the river
            foreach (var river in this.rivers)
            {
                // walk the river from the start point, till we meet the end point (another river, a lake, or the ocean)
                // if the elevation changes are invalid (it ever flows up) correct that.
                // make sure the elevation of all nearby cells is at least a bit higher than this one
                var segment = new List<BiomePolygon>();
                segment.Add(river[0]);
                for (var i = 1; i < river.Count; i++)
                {
                    var polygon = river[i];
                    segment.Add(polygon);
                    if (i == river.Count - 1 ||
                        polygon.HasLake ||
                        polygon.Biome == Biome.Ocean)
                    {
                        // process this segment
                        var startElevation = segment[0].Elevation;
                        var endElevation = Math.Max(0f, segment[segment.Count - 1].Elevation);
                        var maxChange = (startElevation - endElevation) / segment.Count;
                        var currentElevation = endElevation;

                        // walk from the min elevation, easier to correct differences
                        for (var j = segment.Count - 1; j >= 0; j--)
                        {
                            if (!(segment[j].HasLake || segment[j].Biome == Biome.Ocean))
                            {
                                if (segment[j].Elevation < currentElevation)
                                    segment[j].Elevation = currentElevation + ((float)this.rand.NextDouble() * maxChange);
                            }

                            currentElevation = segment[j].Elevation;
                        }

                        segment.Clear();
                    }
                }
            }

            // river cleanup part two - make sure all biomes are marked as having river correctly
            foreach (var poly in this.polygons) poly.HasRiver = false;
            foreach (var cell in this.rivers.SelectMany(river => river)) cell.HasRiver = true;

            // SMOOTH PASS 2 (form valleys around rivers and lakes - only polygons that are near one)
            {
                const int waterSmoothRadius = 3;

                // polygons that are near lake or river
                var riverLakePolys = new HashSet<BiomePolygon>(this.polygons.Where(p => p.HasRiver || p.HasLake));
                var nearWater = this.polygons.Where(p =>
                    !p.HasRiver &&
                    !p.HasLake &&
                    p.Biome != Biome.Ocean &&
                    p.Biome != Biome.DeepOcean &&
                    this.polygons.AdjacentPolygons(p.Index, waterSmoothRadius)
                        .Any(a => riverLakePolys.Contains(this.polygons[a]))).ToArray();

                for (var smoothPass = 1; smoothPass <= 4; smoothPass++)
                {
                    // average the elevations
                    var smoothedHeight = new Dictionary<BiomePolygon, float>();
                    foreach (var p in nearWater)
                    {
                        var adjacent = this.polygons
                            .AdjacentPolygons(p.Index, smoothPass * 2)
                            .Where(i => this.polygons[i].Biome.IsLand() && (this.polygons[i].HasLake || this.polygons[i].HasRiver));
                        if (!adjacent.Any())
                            continue;

                        // smooth downwards to near the lake/river elevation
                        var averageWaterElevation = adjacent.Average(a => this.polygons[a].Elevation);
                        var diff = averageWaterElevation - p.Elevation;
                        var target = p.Elevation + (diff * .2f);

                        smoothedHeight[p] = target;
                    }

                    foreach (var pair in smoothedHeight)
                    {
                        if (pair.Key.Adjacent.Any(p => this.polygons[p].HasRiver || this.polygons[p].HasLake))
                        {
                            var minElevation = .01f + pair.Key.Adjacent.Where(p => this.polygons[p].HasRiver || this.polygons[p].HasLake).Max(p => this.polygons[p].Elevation);
                            pair.Key.Elevation = Math.Max(minElevation, pair.Value);
                        }
                        else
                            pair.Key.Elevation = pair.Value;
                    }
                }
            }

            if (!this.SkipSetSpawnLocation) {
                // set spawn location as the farthest position inside of the grasslands
                var spawnPolygon = this.polygons
                    .Where(p => p.Biome.IsGrassland() && !p.HasRiver && !p.HasLake)
                    .OrderByDescending(p => this.polygons.DistanceTo(p.Index, p2 => p2.Biome.IsGrassland()))
                    .FirstOrDefault() ??
                    this.polygons.Random();
                WorldGeneratorPlugin.SetSpawnLocation(new Vector3i((int)Math.Round(spawnPolygon.Center.X), 0, (int)Math.Round(spawnPolygon.Center.Y)));
            }

            // set coast to warm/cold based on temp
            foreach (var polygon in this.polygons.Where(p => p.Biome == Biome.Coast))
            {
                if (polygon.Temperature > .5f)
                    polygon.Biome = Biome.WarmCoast;
                else
                    polygon.Biome = Biome.ColdCoast;
                polygon.Elevation = .01f; // force coast to be at ocean elevation
            }

            this.RenderTerrainMap(config);

            if (!this.PreviewOnly)
            {
                this.RenderMaps(config);
                this.HeightData      = this.ProcessBitmap(this.HeightMap);
                this.WaterData       = this.ProcessBitmap(this.WaterLevelMap);
                this.RainfallData    = this.polygons.GenerateDataByPolygons(this.WorldSize, x => x.Moisture);
                this.TemperatureData = this.polygons.GenerateDataByPolygons(this.WorldSize, x => x.Temperature);
            }
        }

        public string GetCategory()
        {
            return "";
        }

        public string GetStatus()
        {
            return "";
        }
        
        private void GenerateBiome(Biome targetBiome, Biome placementBiome, Func<BiomePolygon, float> biomePrioritizer, float biomePercent, int numBiomes, Perlin biomeNoise, bool validateLocation = true, bool contiguous = true)
        {
            if (numBiomes == 0 || biomePercent < 0.001) return;
            var remainingBiomeArea = biomePercent;
            var approximateBiomeSize = biomePercent / numBiomes;
            var polygonCountsList = new Dictionary<BiomePolygon, float>();
            bool perfectlyBalanced;
            do
            {
                biomeNoise.Seed = this.rand.Next();
                var forestSize = Math.Min(remainingBiomeArea, approximateBiomeSize * (((float)this.rand.NextDouble() * .5f) + .75f));
                var validPositions = validateLocation ? targetBiome.GetValidPositions(this.polygons) : null;
                perfectlyBalanced = this.BalanceBiome(this.polygons, forestSize, targetBiome, p => p.Biome == placementBiome && (validPositions == null || validPositions.Contains(p)), biomePrioritizer, contiguous);
                remainingBiomeArea = biomePercent - (this.polygons.Count(p => p.Biome == targetBiome) / (float)this.polygons.Length);

                // Add newly added biomes
                var newPolygons = this.polygons.Where(poly => poly.Biome == targetBiome && polygonCountsList.All(existingPoly => existingPoly.Key != poly)).ToList();
                if (newPolygons.Count > 0)
                {
                    var count = newPolygons.Count / (float) this.polygons.Length;
                    foreach (var polygon in newPolygons) polygonCountsList.Add(polygon, count);
                }
            }
            while (perfectlyBalanced && remainingBiomeArea > 0f);

            // Group new polygons by descending and clear biomes which are more than needed count
            polygonCountsList.GroupBy(polygonCounts => polygonCounts.Value).OrderByDescending(polygonCounts => polygonCounts.Key).Skip(numBiomes)
                .ForEach(polygonCountGroup => polygonCountGroup
                    .ForEach(polygonCount => polygonCount.Key.Biome = polygonCount.Key.PreviousBiome));
            polygonCountsList.Clear();
        }
        
        void RenderTerrainMap(VoronoiWorldGeneratorConfig config)
        {
            float fudgeFactor = 1f;
            var bitmap = new DirectBitmap(this.WorldSize, this.WorldSize);
            using (var g = Graphics.FromImage(bitmap))
            {
                this.DrawTerrain(fudgeFactor, g);

                // read from the generated map to get the biomes at each x,y pos, used later for underground gen
                Biome.BiomeData = new Array2D<Biome>(new Vector2i(bitmap.Width, bitmap.Height));
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        while (!Biome.BiomeLookupFromColor.ContainsKey(pixel.ToArgb()))
                        {
                            // ensure pixel has biome data, redrawing with larger error if necessary
                            fudgeFactor += .1f;
                            this.DrawTerrain(fudgeFactor, g);
                            pixel = bitmap.GetPixel(x, y);
                        }
                        Biome.BiomeData[x, y] = Biome.BiomeLookupFromColor[pixel.ToArgb()];
                    }
                }

                // rivers
                var riverPen = new Pen(Color.SteelBlue, config.PointRadius);
                foreach (var river in this.rivers)
                {
                    for (int x = -this.WorldSize; x <= this.WorldSize; x += this.WorldSize)
                        for (int y = -this.WorldSize; y <= this.WorldSize; y += this.WorldSize)
                        {
                            var points = river.Select(r => new PointF(x + r.Center.X, y + r.Center.Y)).ToArray();
                            for (int i = 0; i < points.Length - 1; i++)
                                points[i + 1] = ClosestWrappedLocation(points[i], points[i + 1], this.WorldSize);

                            g.DrawCurve(riverPen, points);
                        }
                }
            }

            this.TerrainMap = bitmap;

            if (config.SaveTerrainImage)
            {
                Image img = new Bitmap(bitmap);
                img.RotateFlip(RotateFlipType.Rotate180FlipX);
                img.Save("Biomes " + config.Seed + ".png");
            }
        }
        
        private bool GenerateCustomWorld(VoronoiWorldGeneratorConfig config)
        {
            Log.WriteLine(Localizer.Do($"Custom World Generator - Feature activated, loading Biomes file: {CustomWorldGeneratorBiomesImageName}"));

            try
            {
                Image biomeImg = Image.FromFile(CustomWorldGeneratorBiomesImageName);
                // Necessary flip to ensure the generated world will be the same as the image
                biomeImg.RotateFlip(RotateFlipType.Rotate180FlipX);

                Log.WriteLine(Localizer.Do($"Custom World Generator - Generating world with seed {config.Seed}"));

                this.ContributeBiomesToPolygons(biomeImg);
            }
            catch (FileNotFoundException)
            {
                Log.WriteWarningLine(Localizer.NotLocalized($"Custom World Generator - Unable to find Biomes image. Falling back to standard generation."));

                return false;
            }

            try
            {
                Image waterImg = Image.FromFile(CustomWorldGeneratorWaterImageName);
                waterImg.RotateFlip(RotateFlipType.Rotate180FlipX);

                this.ContributeRiversAndLakesToPolygons(waterImg);
            }
            catch (FileNotFoundException)
            {
                Log.WriteWarningLine(Localizer.NotLocalized($"Custom World Generator - Unable to find Water image. Generating standard rivers / lakes."));
            }

            this.DoVanillaStuff(config);

            return true;
        }

        private void ContributeBiomesToPolygons(Image img)
        {
            Bitmap bitmap = new Bitmap(img);

            foreach (BiomePolygon p in this.polygons)
            {
                Color pixel = bitmap.GetPixel(Math.Clamp((int)p.Center.X, 0, bitmap.Width - 1), Math.Clamp((int)p.Center.Y, 0, bitmap.Height - 1));

                if (Biome.BiomeLookupFromColor.ContainsKey(pixel.ToArgb()))
                {
                    p.Biome = Biome.BiomeLookupFromColor.GetValueOrDefault(pixel.ToArgb());
                }
                else
                {
                    Log.WriteWarningLine(Localizer.NotLocalized($"Color #{pixel.R.ToString("X2")}{pixel.G.ToString("X2")}{pixel.B.ToString("X2")} at position {(int)p.Center.X},{(int)p.Center.Y} does not have a linked biome. Falling back to Grassland."));
                    p.Biome = Biome.Grassland;
                }

                if (p.Biome == Biome.ColdCoast || p.Biome == Biome.WarmCoast)
                {
                    p.Biome = Biome.Coast;
                }
            }
        }

        private void ContributeRiversAndLakesToPolygons(Image img)
        {
            Bitmap bitmap = new Bitmap(img);

            foreach (BiomePolygon p in this.polygons)
            {
                Color pixel = bitmap.GetPixel(Math.Clamp((int)p.Center.X, 0, bitmap.Width - 1), Math.Clamp((int)p.Center.Y, 0, bitmap.Height - 1));

                if (pixel.ToArgb() == Biome.Ocean.Color.ToArgb())
                {
                    p.HasLake = true;
                }
                else if (pixel.ToArgb() == Biome.DeepOcean.Color.ToArgb())
                {
                    p.HasRiver = true;
                }
                else
                {
                    if (pixel.ToArgb() != 0) {
                        Log.WriteWarningLine(Localizer.NotLocalized($"Color #{pixel.R.ToString("X2")}{pixel.G.ToString("X2")}{pixel.B.ToString("X2")} at position {(int)p.Center.X},{(int)p.Center.Y} is not supported by Water Map."));
                    }
                }
            }
        }

        public void DoVanillaStuff(VoronoiWorldGeneratorConfig config)
        {
            /// ELEVATION, TEMPERATURE, MOISTURE //////////////////////////////////////////////////////////////
            // after biomes are assigned, do height, temperature, moisture

            // first set maximum height for each polygon based on distance to the ocean
            foreach (var p in this.polygons.Where(p => p.Biome.IsLand()))
            {
                float d = this.polygons.DistanceToOcean(p.Index);
                p.MaxElevation = Eco.Shared.Mathf.Pow(d * (1f / config.MaxElevationOceanDistance), config.ElevationPower).Clamp(0f, 1f);
            }

            // use simple noise to set initial heights, use to randomize around biome height range
            var elevationNoiseModule = new RidgedMulti() { Seed = this.rand.Next(), Frequency = 6f * config.InverseScaleModifier };
            var scaleBias = new ScaleBias() { Source0 = elevationNoiseModule, Scale = .5f, Bias = .5f };    // scale/bias to 0-1
            var elevationNoise = new SeamlessNoise() { Source = scaleBias };                                // make it tile

            var heightNoise = new Perlin() { Seed = this.rand.Next(), Frequency = 10f * config.InverseScaleModifier };  // [-1, 1]
            var seamlessHeightNoise = new SeamlessNoise() { Source = heightNoise };

            var moistureNoiseModule = new Perlin() { Seed = this.rand.Next(), Frequency = 5f * config.InverseScaleModifier };
            scaleBias = new ScaleBias() { Source0 = moistureNoiseModule, Scale = .5f, Bias = .5f };         // scale/bias to 0-1
            var moistureNoise = new SeamlessNoise() { Source = scaleBias };                                 // make it tile

            var temperatureNoiseModule = new Perlin() { Seed = this.rand.Next(), Frequency = 5f * config.InverseScaleModifier };
            scaleBias = new ScaleBias() { Source0 = temperatureNoiseModule, Scale = .5f, Bias = .5f };      // scale/bias to 0-1
            var temperatureNoise = new SeamlessNoise() { Source = scaleBias };                              // make it tile

            // randomize height values within the biome range, then blur a bit
            foreach (var p in this.polygons)
            {
                var elevationMod = (float)elevationNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize); // [0, 1]
                var heightMod = (float)seamlessHeightNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);
                var moistureMod = (float)moistureNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);
                var tempMod = (float)temperatureNoise.GetValue(p.Center.X / this.WorldSize, 0, p.Center.Y / this.WorldSize);

                var avgElevation = p.Biome.ElevationRange.Mid;
                var startElevation = avgElevation * elevationMod;
                var mod = heightMod * (p.Biome.ElevationRange.Diff * .5f); // multiply half the diff by -1, 1 to map to -diff, +diff

                var height = startElevation + mod;
                var temperature = p.Biome.TemperatureRange.min + (tempMod * p.Biome.TemperatureRange.Diff);
                var moisture = p.Biome.MoistureRange.min + (moistureMod * p.Biome.MoistureRange.Diff);

                p.Elevation = p.Biome.IsLand() ? height.Clamp(.05f, 1f) : height.Clamp(-1f, -.05f);
                p.Temperature = temperature;
                p.Moisture = moisture;
            }

            // special case coastline - set them to average temp/moisture of cells nearby that aren't coast/ocean
            foreach (var p in this.polygons.Where(p => p.Biome == Biome.Coast))
            {
                var nearby = this.polygons.AdjacentPolygons(p.Index, config.CoastlineSize).Select(i => this.polygons[i]).Where(n => n.Biome != Biome.Coast && n.Biome.IsLand());
                if (nearby.Any())
                {
                    var avgTemp = nearby.Average(n => n.Temperature);
                    var avgMoisture = nearby.Average(n => n.Moisture);
                    p.Temperature = avgTemp;
                    p.Moisture = avgMoisture;
                }
            }

            foreach (var p in this.polygons)
            {
                if (p.Biome == Biome.Grassland)
                {
                    p.Biome = Biome.Grassland;
                }
            }

            // Detect groups of polygons forming lakes
            List<BiomePolygon> seenPolygons = new List<BiomePolygon>();
            this.lakes = new List<List<BiomePolygon>>();

            foreach (var polygon in this.polygons.Where(p => p.HasLake))
            {
                if (seenPolygons.Contains(polygon))
                {
                    continue;
                }

                List<BiomePolygon> lakePolygons = this.FindConnectedAdjacents(this.polygons, polygon, biomePolygon => biomePolygon.HasLake).ToList();

                lakePolygons.ForEach(p => seenPolygons.Add(p));

                this.lakes.Add(lakePolygons);
            }

            Log.WriteLine(Localizer.NotLocalized($"Detected {this.lakes.Count} lakes"));

            // Detect groups of polygons forming rivers, and order them
            List<BiomePolygon> seenPolygonsRivers = new List<BiomePolygon>();
            List<List<BiomePolygon>> unorderedRivers = new List<List<BiomePolygon>>();

            // Detect groups of rivers
            foreach (var poly in this.polygons.Where(p => p.HasRiver))
            {
                if (seenPolygonsRivers.Contains(poly))
                {
                    continue;
                }

                List<BiomePolygon> riverPolygons = this.FindConnectedAdjacents(this.polygons, poly, biomePolygon => biomePolygon.HasRiver).ToList();

                unorderedRivers.Add(riverPolygons);

                foreach (var riverPoly in riverPolygons)
                {
                    seenPolygonsRivers.Add(riverPoly);
                }
            }

            Log.WriteLine(Localizer.NotLocalized($"Detected {unorderedRivers.Count} rivers"));

            // Find, for each river, the best ending of the river
            List<BiomePolygon> endingPoints = new List<BiomePolygon>();
            var riverIndex = 1;

            foreach (var river in unorderedRivers)
            {
                List<BiomePolygon> polyCandidates = new List<BiomePolygon>();

                foreach (var poly in river)
                {
                    if ((poly.Biome == Biome.Ocean || poly.Biome == Biome.DeepOcean)
                        && poly.Adjacent.Select(a => this.polygons[a]).Count(p => p.Biome != Biome.Ocean && p.Biome != Biome.DeepOcean && p.HasRiver) > 0)
                    {
                        polyCandidates.Add(poly);
                    }

                    foreach (var adj in poly.Adjacent.Select(a => this.polygons[a]))
                    {
                        if (adj.HasLake)
                        {
                            polyCandidates.Add(adj);

                            break;
                        }
                    }
                }

                if (polyCandidates.Count == 0)
                {
                    Log.WriteWarningLine(Localizer.NotLocalized($"Unable to find the end of river {riverIndex} !"));
                }
                else
                {
                    var candidatesOcean = polyCandidates.Where(p => p.Biome == Biome.Ocean).ToList();

                    if (candidatesOcean.Count > 0)
                    {
                        /*
                        if (candidatesOcean.Count > 1)
                        {
                            // To check if this is a delta, check if all ending positions are near each others (in the same ocean/deepocean)
                            // if not, it's a delta
                            // doing the delta thing is tricky, needs more investigation
                        }
                        */

                        Log.WriteLine(Localizer.NotLocalized($"River {riverIndex} ends in the ocean."));

                        endingPoints.Add(candidatesOcean[0]);
                    }
                    else
                    {
                        var bestLake = polyCandidates.Where(p => p.HasLake).MinObj(p => p.Elevation);
                        bestLake.HasRiver = true;

                        Log.WriteLine(Localizer.NotLocalized($"River {riverIndex} ends in lake {this.lakes.FindIndex(l => l.Contains(bestLake)) + 1}."));

                        endingPoints.Add(bestLake);
                    }
                }

                riverIndex++;
            }

            Log.WriteLine(Localizer.NotLocalized($"Detected {endingPoints.Count} river endpoints"));

            // Detect the river, from end to start, trying to be the longest without taking adjacent of an existing poly
            this.rivers = new List<List<BiomePolygon>>();

            foreach (var startingPoly in endingPoints)
            {
                List<List<BiomePolygon>> finishedRivers = new List<List<BiomePolygon>>();

                // TODO: find a better shit
                // Here: we need to estimate all potential rivers, and then choose between these rivers.
                for (int i = 0; i < 8192; i++)
                {
                    List<BiomePolygon> potentialRiver = new List<BiomePolygon>() { startingPoly };

                    while (true)
                    {
                        List<BiomePolygon> correctAdjacents = new List<BiomePolygon>();

                        foreach (var adj in potentialRiver.Last().Adjacent.Select(a => this.polygons[a]))
                        {
                            if (!(adj.HasRiver || adj.HasLake) || adj.Biome == Biome.Ocean || adj.Biome == Biome.DeepOcean)
                            {
                                continue;
                            }

                            if (potentialRiver.Contains(adj))
                            {
                                continue;
                            }

                            if (potentialRiver.Take(potentialRiver.Count - 1).SelectMany(r => r.Adjacent.Select(a => this.polygons[a])).Contains(adj))
                            {
                                continue;
                            }

                            correctAdjacents.Add(adj);
                        }

                        if (correctAdjacents.Count == 0)
                        {
                            finishedRivers.Add(potentialRiver);

                            break;
                        }

                        var randomPoly = correctAdjacents[this.rand.Next(0, correctAdjacents.Count)];
                        potentialRiver.Add(randomPoly);

                        if (randomPoly.HasLake)
                        {
                            finishedRivers.Add(potentialRiver);
                            break;
                        }
                    }
                }

                var lakeRivers = finishedRivers.Where(r => r.Last().HasLake && r.Count > 3).ToList();

                List<BiomePolygon> river;

                if (lakeRivers.Count > 0)
                {
                    river = lakeRivers.MinObj(r => r.Count);
                }
                else
                {
                    river = finishedRivers.MaxObj(r => r.Count);
                }

                river.Reverse();
                this.rivers.Add(river);
            }

            // Remove too small rivers
            this.rivers = this.rivers.Where(r => r.Count > 2).ToList();

            Log.WriteLine(Localizer.NotLocalized($"Finalized {this.rivers.Count} rivers"));

            // river cleanup - make sure all biomes are marked as having river correctly
            this.polygons.ForEach(p => p.HasRiver = false);
            this.rivers.SelectMany(river => river).ForEach(p => p.HasRiver = true);

            this.ClearRiversElevation(this.rivers.Where(r => !r.First().HasLake).ToList());

            this.ClearLakesElevation(this.lakes);

            // SMOOTH PASS 2 (form valleys around rivers and lakes - only polygons that are near one)
            {
                const int waterSmoothRadius = 3;

                // polygons that are near lake or river
                var riverLakePolys = new HashSet<BiomePolygon>(this.polygons.Where(p => p.HasRiver || p.HasLake));
                var nearWater = this.polygons.Where(p =>
                    !p.HasRiver &&
                    !p.HasLake &&
                    p.Biome != Biome.Ocean &&
                    p.Biome != Biome.DeepOcean &&
                    this.polygons.AdjacentPolygons(p.Index, waterSmoothRadius)
                        .Any(a => riverLakePolys.Contains(this.polygons[a]))).ToArray();

                for (var smoothPass = 1; smoothPass <= 4; smoothPass++)
                {
                    // average the elevations
                    var smoothedHeight = new Dictionary<BiomePolygon, float>();
                    foreach (var p in nearWater)
                    {
                        var adjacent = this.polygons
                            .AdjacentPolygons(p.Index, smoothPass * 2)
                            .Where(i => this.polygons[i].Biome.IsLand() && (this.polygons[i].HasLake || this.polygons[i].HasRiver));
                        if (!adjacent.Any())
                            continue;

                        // smooth downwards to near the lake/river elevation
                        var averageWaterElevation = adjacent.Average(a => this.polygons[a].Elevation);
                        var diff = averageWaterElevation - p.Elevation;
                        var target = p.Elevation + (diff * .2f);

                        smoothedHeight[p] = target;
                    }

                    foreach (var pair in smoothedHeight)
                    {
                        if (pair.Key.Adjacent.Any(p => this.polygons[p].HasRiver || this.polygons[p].HasLake))
                        {
                            var minElevation = .01f + pair.Key.Adjacent.Where(p => this.polygons[p].HasRiver || this.polygons[p].HasLake).Max(p => this.polygons[p].Elevation);
                            pair.Key.Elevation = Math.Max(minElevation, pair.Value);
                        }
                        else
                            pair.Key.Elevation = pair.Value;
                    }
                }
            }

            // set spawn location as the farthest position inside of the grasslands
            var spawnPolygon = this.polygons
                                   .Where(p => p.Biome.IsGrassland() && !p.HasRiver && !p.HasLake)
                                   .OrderByDescending(p => this.polygons.DistanceTo(p.Index, p2 => p2.Biome.IsGrassland()))
                                   .FirstOrDefault() ??
                               this.polygons.Random();
            WorldGeneratorPlugin.SetSpawnLocation(new Vector3i((int)Math.Round(spawnPolygon.Center.X), 0, (int)Math.Round(spawnPolygon.Center.Y)));

            // set coast to warm/cold based on temp
            foreach (var polygon in this.polygons.Where(p => p.Biome == Biome.Coast))
            {
                if (polygon.Temperature > .5f)
                    polygon.Biome = Biome.WarmCoast;
                else
                    polygon.Biome = Biome.ColdCoast;
                polygon.Elevation = .01f; // force coast to be at ocean elevation
            }

            this.RenderTerrainMap(config);

            if (!this.PreviewOnly)
            {
                this.RenderMaps(config);

                try
                {
                    Image imgH = Image.FromFile(CustomWorldGeneratorHeightmapImageName);
                    imgH.RotateFlip(RotateFlipType.Rotate180FlipX);

                    Log.WriteLine(Localizer.NotLocalized($"Loading {CustomWorldGeneratorHeightmapImageName}"));

                    this.HeightMap = new Bitmap(imgH);
                }
                catch (Exception)
                {
                    Log.WriteWarningLine(Localizer.NotLocalized($"No HeightMap found"));

                    if (SaveTerrainImage)
                    {
                        Bitmap heightmapOutput = new Bitmap(this.HeightMap);
                        heightmapOutput.RotateFlip(RotateFlipType.Rotate180FlipX);
                        heightmapOutput.Save("HeightMap-" + config.Seed + ".png");

                        Log.WriteLine(Localizer.NotLocalized($"Saving HeightMap-{config.Seed}.png"));
                    }
                }

                this.HeightData      = this.ProcessBitmap(this.HeightMap);
                this.WaterData       = this.ProcessBitmap(this.WaterLevelMap);
                this.RainfallData    = this.polygons.GenerateDataByPolygons(this.WorldSize, x => x.Moisture);
                this.TemperatureData = this.polygons.GenerateDataByPolygons(this.WorldSize, x => x.Temperature);
            }
        }
        
        private void ClearRiversElevation(List<List<BiomePolygon>> rivers)
        {
            foreach (var river in rivers)
            {
                this.ClearRiverElevation(river);
            }
        }

        private void ClearRiverElevation(List<BiomePolygon> river)
        {
            float previousElevation = river[0].Elevation;

            foreach (var polygon in river)
            {
                if (polygon.Elevation > previousElevation)
                {
                    polygon.Elevation = previousElevation;
                }
                // Smooth waterfall
                else if (polygon.Elevation < previousElevation - 0.06f && polygon.Elevation >= 0.05f)
                {
                    polygon.Elevation = previousElevation - 0.06f;
                }

                if (polygon.Elevation > 0f && polygon.Elevation < 0.07f && river.Last().HasLake)
                {
                    polygon.Elevation = 0.07f;
                }

                previousElevation = polygon.Elevation;

                foreach (var adj in polygon.Adjacent.Select(a => this.polygons[a]).Where(p => !p.HasRiver
                                                                                                                       && !p.HasLake
                                                                                                                       && p.Biome != Biome.DeepOcean
                                                                                                                       && p.Biome != Biome.Ocean
                                                                                                                       && p.Biome != Biome.Coast))
                {
                    if (adj.Elevation < polygon.Elevation + 0.01f)
                    {
                        adj.Elevation = polygon.Elevation + 0.01f;
                    }
                }
            }

            if (river.Last().HasLake)
            {
                this.ClearLakeElevation(
                    this.lakes.Find(l => l.Contains(river.Last())),
                    river.Last().Elevation - 0.01f,
                    river.Last()
                );
            }
        }

        private void ClearLakesElevation(List<List<BiomePolygon>> lakes)
        {
            foreach (var lake in lakes)
            {
                this.ClearLakeElevation(lake, 0f, null);
            }
        }

        private void ClearLakeElevation(List<BiomePolygon> lake, float forcedElevation, BiomePolygon riverArrival)
        {
            float averageLakeHeight;

            if (forcedElevation > 0f)
            {
                averageLakeHeight = forcedElevation;
            }
            else
            {
                averageLakeHeight = Math.Max(lake.Average(p => p.Elevation), 0.06f);
            }

            // Ensure all lake polygons are at same elevation
            lake.ForEach(p => p.Elevation = averageLakeHeight);

            // Ensure polygons near lake, but not rivers, are higher than lake elevation
            lake
                .ForEach(p => p.Adjacent
                    .Select(a => this.polygons[a])
                    .Where(a => !a.HasLake && !a.HasRiver)
                    .ForEach(a => a.Elevation = averageLakeHeight + 0.01f)
                );

            lake
                .Where(p => p.HasRiver && p != riverArrival)
                .ForEach(p =>
                {
                    var riverFound = this.rivers.Find(r => r.First() == p);

                    if (riverFound != null)
                    {
                        int iii = 0;
                        while (riverFound[iii].HasLake)
                        {
                            iii++;
                        }

                        riverFound[iii].Elevation = averageLakeHeight - 0.01f;

                        this.ClearRiverElevation(riverFound);
                    }
                });
        }
        
        private void DrawTerrain(float fudgeFactor, Graphics g)
        {
            foreach (var polygon in this.polygons)
            {
                var points = polygon.Points.Select(pt =>
                {
                    float dx = pt.X - polygon.Center.X;
                    float dy = pt.Y - polygon.Center.Y;
                    Vector2 v = new Vector2(dx, dy).Normalized * fudgeFactor;
                    return new PointF(polygon.Center.X + dx + v.x, polygon.Center.Y + dy + v.y);
                }).ToArray();

                // fill the polygon
                g.FillPolygon(polygon.Brush, points);

                bool leftBorder = points.Any(p => p.X < 0);
                bool rightBorder = points.Any(p => p.X >= this.WorldSize);
                bool topBorder = points.Any(p => p.Y < 0);
                bool bottomBorder = points.Any(p => p.Y >= this.WorldSize);

                bool topLeft = points.Any(p => p.X < 0 && p.Y < 0);
                bool topRight = points.Any(p => p.X >= this.WorldSize && p.Y < 0);
                bool bottomLeft = points.Any(p => p.X < 0 && p.Y >= this.WorldSize);
                bool bottomRight = points.Any(p => p.X >= this.WorldSize && p.Y >= this.WorldSize);

                // till fill to get all the edges (since we wrap)
                if (leftBorder)     g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X + this.WorldSize, p.Y)).ToArray());
                if (rightBorder)    g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X - this.WorldSize, p.Y)).ToArray());
                if (topBorder)      g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X, p.Y + this.WorldSize)).ToArray());
                if (bottomBorder)   g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X, p.Y - this.WorldSize)).ToArray());

                if (topLeft)        g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X + this.WorldSize, p.Y + this.WorldSize)).ToArray());
                if (topRight)       g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X - this.WorldSize, p.Y + this.WorldSize)).ToArray());
                if (bottomLeft)     g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X + this.WorldSize, p.Y - this.WorldSize)).ToArray());
                if (bottomRight)    g.FillPolygon(polygon.Brush, points.Select(p => new PointF(p.X - this.WorldSize, p.Y - this.WorldSize)).ToArray());
            }
        }

        public void RenderMaps(VoronoiWorldGeneratorConfig config)
        {
            // height maps (land + water)
            var worldSize = this.WorldSize;
            using (var heightMap = new DirectBitmap(worldSize, worldSize))
            {
                // draw elevations into heightmap
                using (var hg = Graphics.FromImage(heightMap))
                {
                    hg.SmoothingMode = SmoothingMode.None;
                    hg.Clear(Color.Black);

                    foreach (var polygon in this.polygons)
                    {
                        using (var brush = new SolidBrush(polygon.HeightmapColor))
                        {
                            hg.FillPolygon(brush, polygon.Points);

                            bool leftBorder = polygon.Points.Any(p => p.X < 0);
                            bool rightBorder = polygon.Points.Any(p => p.X >= worldSize);
                            bool topBorder = polygon.Points.Any(p => p.Y < 0);
                            bool bottomBorder = polygon.Points.Any(p => p.Y >= worldSize);

                            bool topLeft = polygon.Points.Any(p => p.X < 0 && p.Y < 0);
                            bool topRight = polygon.Points.Any(p => p.X >= worldSize && p.Y < 0);
                            bool bottomLeft = polygon.Points.Any(p => p.X < 0 && p.Y >= worldSize);
                            bool bottomRight = polygon.Points.Any(p => p.X >= worldSize && p.Y >= worldSize);

                            // till fill to get all the edges (since we wrap)
                            if (leftBorder)     hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X + worldSize, p.Y)).ToArray());
                            if (rightBorder)    hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X - worldSize, p.Y)).ToArray());
                            if (topBorder)      hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X, p.Y + worldSize)).ToArray());
                            if (bottomBorder)   hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X, p.Y - worldSize)).ToArray());

                            if (topLeft)        hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X + worldSize, p.Y + worldSize)).ToArray());
                            if (topRight)       hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X - worldSize, p.Y + worldSize)).ToArray());
                            if (bottomLeft)     hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X + worldSize, p.Y - worldSize)).ToArray());
                            if (bottomRight)    hg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X - worldSize, p.Y - worldSize)).ToArray());
                        }
                    }
                }

                // draw water map and water elevation map
                using (var waterBitmap = new DirectBitmap(worldSize, worldSize))
                using (var waterElevationBitmap = new DirectBitmap(worldSize, worldSize))
                {
                    // draw water features
                    using (var wg = Graphics.FromImage(waterBitmap))
                    using (var weg = Graphics.FromImage(waterElevationBitmap))
                    {
                         wg.Clear(Color.Black);
                         weg.Clear(Color.Black);

                         // rivers
                         var waterMarkColor = Color.FromArgb(255, 1, 1, 1);
                         using (var waterPen = new Pen(waterMarkColor, config.RiverCellWidth))
                         {
                             foreach (var river in this.rivers)
                             {
                                 for (int x = -worldSize; x <= worldSize; x += worldSize)
                                 {
                                     for (int y = -worldSize; y <= worldSize; y += worldSize)
                                     {
                                         var points = river.Select(r => new PointF(x + r.Center.X, y + r.Center.Y)).ToArray();
                                         for (int i = 0; i < points.Length - 1; i++)
                                             points[i + 1] = ClosestWrappedLocation(points[i], points[i + 1], worldSize);

                                         if (river.Count > 2)
                                         {
                                             PointF[] segmentPoints = new PointF[3];
                                             for (int segment = 0; segment < (points.Length - 2); segment++)
                                             {
                                                 segmentPoints[0] = points[segment];
                                                 segmentPoints[1] = points[segment + 1];
                                                 segmentPoints[2] = points[segment + 2];

                                                 var e = Math.Max(0, 127 + (int)(128 * river[segment + 1].Elevation));
                                                 var segmentPen = new Pen(Color.FromArgb(255, e, e, e), config.RiverCellWidth + 2f); // account for river banks
                                                 weg.DrawCurve(segmentPen, segmentPoints);
                                             }
                                         }
                                         wg.DrawCurve(waterPen, points);
                                     }
                                 }
                             }
                         }

                        // lakes
                        using (var brush = new SolidBrush(waterMarkColor))
                        {
                            foreach (var polygon in this.polygons.Where(p => p.HasLake))
                            {
                                // fill the polygon
                                bool leftBorder = polygon.Points.Any(p => p.X < 0);
                                bool rightBorder = polygon.Points.Any(p => p.X >= worldSize);
                                bool topBorder = polygon.Points.Any(p => p.Y < 0);
                                bool bottomBorder = polygon.Points.Any(p => p.Y >= worldSize);

                                bool topLeft = polygon.Points.Any(p => p.X < 0 && p.Y < 0);
                                bool topRight = polygon.Points.Any(p => p.X >= worldSize && p.Y < 0);
                                bool bottomLeft = polygon.Points.Any(p => p.X < 0 && p.Y >= worldSize);
                                bool bottomRight = polygon.Points.Any(p => p.X >= worldSize && p.Y >= worldSize);

                                wg.FillPolygon(brush, polygon.Points);

                                // till fill to get all the edges (since we wrap)
                                if (leftBorder)     wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X + worldSize, p.Y)).ToArray());
                                if (rightBorder)    wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X - worldSize, p.Y)).ToArray());
                                if (topBorder)      wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X, p.Y + worldSize)).ToArray());
                                if (bottomBorder)   wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X, p.Y - worldSize)).ToArray());

                                if (topLeft)        wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X + worldSize, p.Y + worldSize)).ToArray());
                                if (topRight)       wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X - worldSize, p.Y + worldSize)).ToArray());
                                if (bottomLeft)     wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X + worldSize, p.Y - worldSize)).ToArray());
                                if (bottomRight)    wg.FillPolygon(brush, polygon.Points.Select(p => new PointF(p.X - worldSize, p.Y - worldSize)).ToArray());
                            }
                        }

                        // lake elevation
                        foreach (var polygon in this.polygons.Where(p => p.HasLake))
                        {
                            // fill the polygon
                            using (var brush = new SolidBrush(polygon.HeightmapColor))
                            {
                                // scale points out a bit from the center, to form the shoreline of the lake
                                var points = polygon.Points.Select(pt =>
                                {
                                    float dx = pt.X - polygon.Center.X;
                                    float dy = pt.Y - polygon.Center.Y;
                                    return new PointF(polygon.Center.X + (dx * 1.5f), polygon.Center.Y + (dy * 1.5f));
                                }).ToArray();

                                bool leftBorder = points.Any(p => p.X < 0);
                                bool rightBorder = points.Any(p => p.X >= worldSize);
                                bool topBorder = points.Any(p => p.Y < 0);
                                bool bottomBorder = points.Any(p => p.Y >= worldSize);

                                bool topLeft = points.Any(p => p.X < 0 && p.Y < 0);
                                bool topRight = points.Any(p => p.X >= worldSize && p.Y < 0);
                                bool bottomLeft = points.Any(p => p.X < 0 && p.Y >= worldSize);
                                bool bottomRight = points.Any(p => p.X >= worldSize && p.Y >= worldSize);

                                weg.FillPolygon(brush, points);

                                // till fill to get all the edges (since we wrap)
                                if (leftBorder)     weg.FillPolygon(brush, points.Select(p => new PointF(p.X + worldSize, p.Y)).ToArray());
                                if (rightBorder)    weg.FillPolygon(brush, points.Select(p => new PointF(p.X - worldSize, p.Y)).ToArray());
                                if (topBorder)      weg.FillPolygon(brush, points.Select(p => new PointF(p.X, p.Y + worldSize)).ToArray());
                                if (bottomBorder)   weg.FillPolygon(brush, points.Select(p => new PointF(p.X, p.Y - worldSize)).ToArray());

                                if (topLeft)        weg.FillPolygon(brush, points.Select(p => new PointF(p.X + worldSize, p.Y + worldSize)).ToArray());
                                if (topRight)       weg.FillPolygon(brush, points.Select(p => new PointF(p.X - worldSize, p.Y + worldSize)).ToArray());
                                if (bottomLeft)     weg.FillPolygon(brush, points.Select(p => new PointF(p.X + worldSize, p.Y - worldSize)).ToArray());
                                if (bottomRight)    weg.FillPolygon(brush, points.Select(p => new PointF(p.X - worldSize, p.Y - worldSize)).ToArray());
                            }
                        }
                    }

                    // smooth out the water elevation bitmap a bit (blur)
                    using (var blurredWater = new DirectBitmap(waterElevationBitmap))
                    {
                        for (int blurPasses = 0; blurPasses < 2; blurPasses++)
                        {
                            const int radius = 2;
                            const int sqRadius = radius * radius;
                            for (int y = 0; y < worldSize; y++)
                            {
                                for (int x = 0; x < worldSize; x++)
                                {
                                    var sourceColor = waterElevationBitmap.GetPixel(x, y);
                                    if (sourceColor.R == 0)
                                        continue; // don't blur areas with no water

                                    int sum = 0;
                                    int count = 0;
                                    for (int y2 = y - radius; y2 <= y + radius; y2++)
                                    {
                                        for (int x2 = x - radius; x2 <= x + radius; x2++)
                                        {
                                            int dx = x2 - x;
                                            int dy = y2 - y;
                                            float sqDistance = (dx * dx) + (dy * dy);
                                            if (sqDistance < sqRadius)
                                            {
                                                int px = (x2 + worldSize) % worldSize;
                                                int py = (y2 + worldSize) % worldSize;
                                                var color = waterElevationBitmap.GetPixel(px, py);
                                                if (color.R != 0)
                                                {
                                                    sum += color.R;
                                                    count++;
                                                }
                                            }
                                        }
                                    }

                                    if (count > 1)
                                    {
                                        int average = (int)Math.Round((double)sum / count);
                                        blurredWater.SetPixel(x, y, Color.FromArgb(255, average, average, average));
                                    }
                                }
                            }

                            Array.Copy(blurredWater.Bits, waterElevationBitmap.Bits, blurredWater.Bits.Length);
                        }
                    }

                    // floodfill to determine depth of water features
                    int pixelsSet;
                    int source = 1;
                    int depthChange = 2; // set to higher to debug
                    do
                    {
                        pixelsSet = 0;
                        var result = Math.Max(1, source + depthChange);

                        List<System.Drawing.Point> setPoints = new List<System.Drawing.Point>();
                        for (int y = 0; y < worldSize; y++)
                        {
                            for (int x = 0; x < worldSize; x++)
                            {
                                bool enclosed = true;
                                for (int y2 = y - 1; y2 <= y + 1 && enclosed; y2++)
                                    for (int x2 = x - 1; x2 <= x + 1 && enclosed; x2++)
                                    {
                                        if (x2 == x && y2 == y)
                                            continue;

                                        int px = (x2 + worldSize) % worldSize;
                                        int py = (y2 + worldSize) % worldSize;

                                        var pixel = waterBitmap.GetPixel(px, py);
                                        if (pixel.G != source)
                                            enclosed = false;
                                    }

                                if (enclosed && waterBitmap.GetPixel(x, y).R != result)
                                {
                                    setPoints.Add(new System.Drawing.Point(x, y));
                                    pixelsSet++;
                                }
                            }
                        }

                        foreach (var entry in setPoints)
                            waterBitmap.SetPixel(entry.X, entry.Y, Color.FromArgb(255, result, result, result));

                        source += depthChange;
                        if (source > 255)
                            source = 255;
                    }
                    while (pixelsSet > 0);

                    // paint the water depth into the heightmap
                    using (var waterResult = new DirectBitmap(worldSize, worldSize))
                    {
                        for (int y = 0; y < worldSize; y++)
                        {
                            for (int x = 0; x < worldSize; x++)
                            {
                                waterResult.SetPixel(x, y, Color.Black);
                                var waterHeight = waterElevationBitmap.GetPixel(x, y).R;
                                if (waterHeight != 0)
                                {
                                    var waterDepth = waterBitmap.GetPixel(x, y).R;
                                    var result = Math.Max(0, waterHeight - waterDepth);
                                    heightMap.SetPixel(x, y, Color.FromArgb(255, result, result, result));

                                    if (waterDepth > 0)
                                        waterResult.SetPixel(x, y, Color.FromArgb(255, waterHeight, waterHeight, waterHeight));
                                }
                            }
                        }
                        this.WaterLevelMap = new Bitmap(waterResult);
                    }

                    // blur the heightmap
                    using (var blurredHeightmap = new DirectBitmap(worldSize, worldSize))
                    {
                        for (int blurPass = 0; blurPass < 2; blurPass++)
                        {
                            const int radius = 4;
                            const int sqRadius = radius * radius;
                            const int threshold = 5;
                            for (int y = 0; y < worldSize; y++)
                            {
                                for (int x = 0; x < worldSize; x++)
                                {
                                    int sum = 0;
                                    int count = 0;
                                    int sourceValue = heightMap.GetPixel(x, y).R;
                                    for (int y2 = y - radius; y2 <= y + radius; y2++)
                                    {
                                        for (int x2 = x - radius; x2 <= x + radius; x2++)
                                        {
                                            int dx = x2 - x;
                                            int dy = y2 - y;
                                            float sqDistance = (dx * dx) + (dy * dy);
                                            if (sqDistance < sqRadius)
                                            {
                                                int px = (x2 + worldSize) % worldSize;
                                                int py = (y2 + worldSize) % worldSize;
                                                var color = heightMap.GetPixel(px, py);
                                                if (Math.Abs(sourceValue - color.R) <= threshold || (sourceValue >= color.R))
                                                {
                                                    sum += color.R;
                                                    count++;
                                                }
                                            }
                                        }
                                    }

                                    int average = (int)Math.Round((double)sum / count);

                                    // if its water, only use the average if its lower than the original value
                                    var water = waterElevationBitmap.GetPixel(x, y).R;
                                    if (water > 0)
                                    {
                                        var sourceColor = heightMap.GetPixel(x, y).R;
                                        average = Math.Min(sourceColor, average);
                                    }
                                    blurredHeightmap.SetPixel(x, y, Color.FromArgb(255, average, average, average));
                                }
                            }

                            Array.Copy(blurredHeightmap.Bits, heightMap.Bits, blurredHeightmap.Bits.Length);
                        }
                    }
                }
                this.HeightMap = new Bitmap(heightMap);
            }
        }

        // takes an existing biome and splits a subsection of it into another biome
        // returns true if at least some biomes were placed down
        bool BalanceBiome(BiomePolygon[] polygons, float desiredPercentage, Biome targetBiome, Predicate<BiomePolygon> selector, Func<BiomePolygon, float> prioritizer, bool contiguous = true)
        {
            var validPolygons = polygons.Where(p => selector(p)).ToArray();
            BiomePolygon sourcePolygon = null;
            float[] priorityLookup = null;
            if (prioritizer == null)
            {
                if (validPolygons.Any())
                    sourcePolygon = validPolygons[this.rand.Next(0, validPolygons.Length)];
            }
            else
            {
                priorityLookup = new float[polygons.Length];
                var highestPriority = float.MinValue;
                foreach (var p in validPolygons)
                {
                    var priority = prioritizer(p);
                    priorityLookup[p.Index] = prioritizer(p);
                    if (priority > highestPriority)
                    {
                        sourcePolygon = p;
                        highestPriority = priority;
                    }
                }
            }

            if (sourcePolygon == null) return false; // no valid locations

            var fillPolygons = new List<BiomePolygon>();
            var visited = new HashSet<BiomePolygon> {sourcePolygon};

            if (!contiguous)
            {
                fillPolygons.AddRange(validPolygons);
                fillPolygons.Sort((a, b) => priorityLookup[b.Index].CompareTo(priorityLookup[a.Index]));
            }
            else fillPolygons.Add(sourcePolygon);

            var currentPercentage = 0f;
            while (currentPercentage < desiredPercentage && fillPolygons.Count > 0)
            {
                // flood fill out our biome till we match the desired percentage, based on prioritizer
                var selectedPolygon = fillPolygons[0];
                if (prioritizer == null)
                {
                    var index = this.rand.Next(0, fillPolygons.Count);
                    selectedPolygon = fillPolygons[index];
                    fillPolygons.RemoveAt(index);
                }
                else
                {
                    selectedPolygon = fillPolygons[0];
                    fillPolygons.RemoveAt(0);
                }

                selectedPolygon.Biome = targetBiome;
                currentPercentage += 1f / polygons.Length;

                var adjacent = selectedPolygon.Adjacent.Select(i => polygons[i]).Where(p => !visited.Contains(p) && selector(p)).ToArray();
                if (adjacent.Any())
                {
                    fillPolygons.AddRange(adjacent);
                    foreach (var a in adjacent) visited.Add(a);

                    if (prioritizer != null) fillPolygons.Sort((a, b) => priorityLookup[b.Index].CompareTo(priorityLookup[a.Index]));
                }
            }

            return true;
        }

        List<GraphEdge> MakeVoronoiGraph(List<PointF> sites, int width, int height)
        {
            Voronoi voroObject = new Voronoi(0.1);
            double[] xVal = new double[sites.Count];
            double[] yVal = new double[sites.Count];
            for (int i = 0; i < sites.Count; i++)
            {
                xVal[i] = sites[i].X;
                yVal[i] = sites[i].Y;
            }
            return voroObject.GenerateVoronoi(xVal, yVal, -width, width * 2, -height, height * 2);
        }

        Array2D<float> ProcessBitmap(Bitmap source)
        {
            if (source == null)
                return null;

            // copy out the stuff
            var result = new Array2D<float>(new Vector2i(source.Width, source.Height));

            for (int y = 0; y < source.Height; y++)
                for (int x = 0; x < source.Width; x++)
                {
                    var color = source.GetPixel(x, y);
                    var value = ((color.R / 255f) * 2f) - 1f; // convert to range [-1f, 1f]
                    result[x, y] = value;
                }

            return result;
        }

        public static PointF ClosestWrappedLocation(PointF viewPosition, PointF position, int worldSize = 500)
        {
            var newPosition = new PointF(position.X, position.Y);
            var halfSize    = new PointF(worldSize * .5f, worldSize * .5f);
            var difference  = new PointF(position.X - viewPosition.X, position.Y - viewPosition.Y);

            if (difference.X < -halfSize.X)
                newPosition.X += worldSize;
            else if (difference.X > halfSize.X)
                newPosition.X -= worldSize;

            if (difference.Y < -halfSize.Y)
                newPosition.Y += worldSize;
            else if (difference.Y > halfSize.Y)
                newPosition.Y -= worldSize;

            return newPosition;
        }
        
        public BiomePolygon[] FindConnectedAdjacents(BiomePolygon[] polygons, BiomePolygon startPoly, Func<BiomePolygon, bool> predicate)
        {
            List<BiomePolygon> matchingPolygons = new List<BiomePolygon>();
            List<BiomePolygon> visited          = new List<BiomePolygon>();
            List<BiomePolygon> polygonsToTest   = new List<BiomePolygon>();

            polygonsToTest.Add(startPoly);

            while (polygonsToTest.Count > 0)
            {
                var poly = polygonsToTest.GetAtIndexOrDefault(0);
                polygonsToTest.RemoveAt(0);

                if (!visited.Contains(poly))
                {
                    matchingPolygons.Add(poly);

                    foreach (var adj in poly.Adjacent.Select(a => polygons[a]).Where(a => !visited.Contains(a) && predicate(a)))
                    {
                        polygonsToTest.Add(adj);
                    }
                }

                visited.Add(poly);
            }

            return matchingPolygons.ToArray();
        }
    }
}
