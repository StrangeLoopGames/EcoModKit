namespace Eco.Mods.TechTree
{
    using Eco.Gameplay.Items;

    public partial class ComputerLabRecipe
    {
        partial void ModsPreInitialize()
        {
            var ingredients = this.Recipes[0].Ingredients;
            ingredients.Clear();
            ingredients.AddRange(
                new IngredientElement[]
                {
                new IngredientElement(typeof(FramedGlassItem), 7500, true), 
                new IngredientElement(typeof(DendrologyResearchPaperBasicItem), 3000, true),
                new IngredientElement(typeof(ModernUpgradeLvl4Item), 250, true)
                });
            this.LaborInCalories = CreateLaborInCaloriesValue(50000, typeof(ElectronicsSkill));
            this.CraftMinutes = CreateCraftTimeValue(1440);
        }
    }
}