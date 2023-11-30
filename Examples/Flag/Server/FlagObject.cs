using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Items;
using Eco.Shared.Localization;
using Eco.Gameplay.Objects;
using Eco.Shared.Serialization;
using Eco.Shared.Math;
using Eco.Core.Items;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Items.Recipes;

namespace Eco.Mods.TechTree
{
    [Serialized]
    [LocDisplayName("Test Flag")]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    public partial class FlagObject : WorldObject, IRepresentsItem
    {
        public    virtual  Type RepresentedItemType => typeof(FlagItem);

        // Runs on object placement/load before its first tick.
        protected override void Initialize()        => base.Initialize();
        protected override void PostInitialize()    => base.PostInitialize();

        // Runs before the destruction of the object in world.
        protected override void OnDestroy()         => base.OnDestroy();

    }

    [Serialized]
    [LocDisplayName("Test Flag")]  // Allows you to change the name the player sees for this item
    [LocDescription("A piece of fabric with something on it. can be used for decorating.")] // The tooltip discription of the item.
    [Ecopedia("Housing Objects", "Flags", createAsSubPage: true, DisplayOnPage = true)] // Creates a new subpage in Flags in the Housing Objects section for this item.
    [Weight(10)]
    public partial class FlagItem : WorldObjectItem<FlagObject>
    {
        static FlagItem()
        {
            // Adds occupancy to the object in all the blocks listed in orentation of the root (front left bottom)
            WorldObject.AddOccupancy<FlagObject>(new List<BlockOccupancy>(){
                new(new Vector3i(0, 0, 0)),
                new(new Vector3i(0, 1, 0)),
            });
        }
    }

    public partial class FlagRecipe : RecipeFamily
    {
        public FlagRecipe()
        {
            this.Initialize(Localizer.DoStr("Flag"), typeof(FlagRecipe));
            this.Recipes = new List<Recipe>
            {
                new Recipe(
                    "Flag",
                    Localizer.DoStr("Flag"),
                    new IngredientElement[]
                    {
                    new IngredientElement("Wood", 10),                    
                    new IngredientElement(typeof(ClothItem), 5)
                    },
                    new CraftingElement[]
                    {
                    new CraftingElement<FlagItem>(),
                    }
                )
            };

            this.LaborInCalories = CreateLaborInCaloriesValue(30);
            this.CraftMinutes = CreateCraftTimeValue(1f);
            this.Initialize(Localizer.DoStr("Flag"), typeof(FlagRecipe));

            CraftingComponent.AddRecipe(typeof(SawmillObject), this);
        }
    }
}