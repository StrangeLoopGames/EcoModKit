namespace Eco.Mods.TechTree
{
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Items;
    using Eco.Shared.Localization;
    using Eco.Gameplay.Objects;
    using Eco.Shared.Serialization;
    using Eco.Shared.Math;
    using System.Collections.Generic;
    using Eco.Core.Items;
    using Eco.Gameplay.Players;


    [Serialized]
    [RequireComponent(typeof(SolidGroundComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    public partial class FlagObject : WorldObject
    {
        public override LocString DisplayName { get { return Localizer.DoStr("Test Flag"); } }
        public bool isRoom { get; set; }
        protected override void Initialize()
        {
          
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
        }

    }

    [Serialized]
    [LocDisplayName("Test Flag")]
    [Ecopedia("Housing Objects", "Flags", createAsSubPage: true, display: InPageTooltip.DynamicTooltip)]
    [Weight(10)]
    public partial class FlagItem : WorldObjectItem<FlagObject>
    {
        public override LocString DisplayDescription { get { return Localizer.DoStr("A piece of fabric with something on it. can be used for decorating."); } }

        static FlagItem()
        {
            WorldObject.AddOccupancy<FlagObject>(new List<BlockOccupancy>(){
            new BlockOccupancy(new Vector3i(0, 0, 0)),
            new BlockOccupancy(new Vector3i(0, 1, 0)),
            });
        }
    }

    public partial class FlagRecipe :
RecipeFamily
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