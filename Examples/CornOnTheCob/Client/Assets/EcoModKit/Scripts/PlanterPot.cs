// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

/// <summary>Client side component for using the server FakePlantComponent. Spawns a fake plant when seeds are stored in a world object "pot".</summary>
/// <inheritdoc/>
public partial class PlanterPot : WorldObject
{
    [Tooltip("Reference to the blockset of plants to search in by species name")]
    /// <summary>Reference to the blockset of plants to search in by species name</summary>
    public BlockSet PlantBlocks;

    [Tooltip("Reference to the blockset of trees to search in by species name")]
    /// <summary>Reference to the blockset of trees to search in by species name</summary>
    public BlockSet TreeBlocks;

    [Tooltip("Transform to spawn the plant at. Offsets for this can be achieved per plant by adding PlanterPotSettings.cs to the plant prefab")]
    /// <summary>Transform to spawn the plant at. Offsets for this can be achieved per plant by adding PlanterPotSettings.cs to the plant prefab</summary>
    public Transform SpawnPoint;
}
