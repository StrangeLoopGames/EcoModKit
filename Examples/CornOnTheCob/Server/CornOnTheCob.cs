using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Gameplay.Skills;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.Mods.TechTree
{
    [Serialized]
    [LocDisplayName("Corn on the cob")]
    [Weight(300)]
    [Tag("BakedVegetable", 1)]
    [Tag("BakedFood", 1)]
    public partial class CornOnTheCobItem : FoodItem
    {
        public override LocString DisplayNamePlural => Localizer.DoStr("Corn on the Cob ");
        public override LocString DisplayDescription => Localizer.DoStr("A warmly colored kernel studded vegetable.");
        public override Nutrients Nutrition => new Nutrients { Carbs = 12, Fat = 2, Protein = 3, Vitamins = 11};

        public override float Calories => 250;
        public override int ShelfLife => 86000;

    }

    [RequiresSkill(typeof(BakingSkill), 1)]
    public partial class CornOnTheCobRecipe : RecipeFamily
    {
        public CornOnTheCobRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "Corn on the cob",  //noloc
                Localizer.DoStr("Corn on the cob"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(CornItem), 4, typeof(BakingSkill), typeof(BakingLavishResourcesTalent)),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<CornOnTheCobItem>(1)
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 1;
            this.LaborInCalories = CreateLaborInCaloriesValue(25, typeof(BakingSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(BakedCornRecipe), 2, typeof(BakingSkill), typeof(BakingFocusedSpeedTalent), typeof(BakingParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Corn on the cob"), typeof(CornOnTheCobRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(BakeryOvenObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }

}
