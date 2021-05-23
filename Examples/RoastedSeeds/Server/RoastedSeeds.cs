namespace Eco
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.EcopediaRoot;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Mods.TechTree;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.View;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [Serialized]
    [LocDisplayName("Roasted Seeds")]
    [Weight(100)]
    [Ecopedia("Food", "Campfire", createAsSubPage: true, display: InPageTooltip.DynamicTooltip)]
    public partial class RoastedSeedsItem : FoodItem
    {
        public override LocString DisplayDescription => Localizer.DoStr("Better for throwing, but edible.");
        public override float Calories => 110;
        public override Nutrients Nutrition => new Nutrients() { Carbs = 7, Fat = 1, Protein = 1, Vitamins = 3 };
    }

    [RequiresSkill(typeof(CampfireCookingSkill), 1)]
    public partial class RoastedSeedsRecipe :
        RecipeFamily
    {
        public RoastedSeedsRecipe()
        {
            var product = new Recipe(
                "Roasted Seeds", 
                Localizer.DoStr("Roasted Seeds"), 
                new IngredientElement[]
                {
                    new IngredientElement(typeof(SunflowerSeedItem), 1, typeof(CampfireCookingSkill))
                },
                new CraftingElement<RoastedSeedsItem>(1)
            );

            this.Recipes = new List<Recipe> { product };
            this.LaborInCalories = CreateLaborInCaloriesValue(20, typeof(CampfireCookingSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(RoastedSeedsRecipe), 2, typeof(CampfireCookingSkill));
            this.Initialize(Localizer.DoStr("Roasted Seeds"), typeof(RoastedSeedsRecipe)); 
            CraftingComponent.AddRecipe(typeof(CampfireObject), this);
        }
    }
}
