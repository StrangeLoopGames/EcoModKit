using Eco.Gameplay.Blocks;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Skills;
using Eco.Core.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.World.Blocks;
using Eco.Gameplay.Items.Recipes;

namespace Eco.Mods.TechTree
{
    [RequiresSkill(typeof(GlassworkingSkill), 1)]
    public partial class GreenStainedGlassRecipe : RecipeFamily
    {
        public GreenStainedGlassRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "Green Stained Glass",
                Localizer.DoStr("Green Stained Glass"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(SandItem), 6, typeof(GlassworkingSkill), typeof(GlassworkingLavishResourcesTalent)),
                    new IngredientElement(typeof(CrushedLimestoneItem), 1, true),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<GreenStainedGlassItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 1;
            this.LaborInCalories = CreateLaborInCaloriesValue(50, typeof(GlassworkingSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(GreenStainedGlassRecipe), 1.5f, typeof(GlassworkingSkill), typeof(GlassworkingFocusedSpeedTalent), typeof(GlassworkingParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Green Stained Glass"), typeof(GreenStainedGlassRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(KilnObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [Solid, Wall, Constructed,BuildRoomMaterialOption]
    [BlockTier(2)]                                          
    [DoesntEncase]                                          
    [RequiresSkill(typeof(GlassworkingSkill), 1)]      
    public partial class GreenStainedGlassBlock : Block, IRepresentsItem   
    {
        public Type RepresentedItemType { get { return typeof(GreenStainedGlassItem); } }  
    }

    [Serialized]
    [LocDisplayName("Green Stained Glass")]
    [LocDescription("A transparent, solid material useful for more than just windows.")]
    [MaxStackSize(20)]                           
    [Weight(10000)]      
    [Ecopedia("Blocks", "Building Materials", createAsSubPage: true, displayOnPage: true)]                    
    [Currency][Tag("Currency")]                              
    [Tag("Constructable")]                         
    [Tier(2)]                                      
    public partial class GreenStainedGlassItem : BlockItem<GreenStainedGlassBlock>
    {
        public override LocString DisplayNamePlural { get { return Localizer.DoStr("Green Stained Glass"); } }

        public override bool CanStickToWalls { get { return false; } }  

        private static Type[] blockTypes = new Type[] {
            typeof(GreenStainedGlassStacked1Block),
            typeof(GreenStainedGlassStacked2Block),
            typeof(GreenStainedGlassStacked3Block),
            typeof(GreenStainedGlassStacked4Block)
        };
        public override Type[] BlockTypes { get { return blockTypes; } }
    }

    [Serialized, Solid]       public class GreenStainedGlassStacked1Block : PickupableBlock { }
    [Serialized, Solid]       public class GreenStainedGlassStacked2Block : PickupableBlock { }
    [Serialized, Solid]       public class GreenStainedGlassStacked3Block : PickupableBlock { }
    [Serialized, Solid, Wall] public class GreenStainedGlassStacked4Block : PickupableBlock { } //Only a wall if it's all 4 Glass

    [Serialized]
    [Wall, Constructed, Solid, BuildRoomMaterialOption]
    [BlockTier(2)]
    [IsForm(typeof(WindowFormType), typeof(GreenStainedGlassItem))]
    public partial class GreenStainedGlassWindowBlock :
        Block, IRepresentsItem
    {
        public Type RepresentedItemType { get { return typeof(GreenStainedGlassItem); } }
    }
    [Serialized]
    [Wall, Constructed, Solid, BuildRoomMaterialOption]
    [BlockTier(2)]
    [IsForm(typeof(CubeFormType), typeof(GreenStainedGlassItem))]
    public partial class GreenStainedGlassCubeBlock :
        Block, IRepresentsItem
    {
        public Type RepresentedItemType { get { return typeof(GreenStainedGlassItem); } }
    }
    [Serialized]
    [Wall, Constructed, Solid, BuildRoomMaterialOption]
    [BlockTier(2)]
    [IsForm(typeof(FlatRoofFormType), typeof(GreenStainedGlassItem))]
    public partial class GreenStainedGlassFlatRoofBlock :
        Block, IRepresentsItem
    {
        public Type RepresentedItemType { get { return typeof(GreenStainedGlassItem); } }
    }
}
