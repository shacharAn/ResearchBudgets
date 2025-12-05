using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.BL
{
    public class BudgetCategoriesBl
    {
        private readonly BudgetCategoriesDal _dal;

        public BudgetCategoriesBl()
        {
            _dal = new BudgetCategoriesDal();
        }

        public List<BudgetCategories> GetAllCategories()
        {
            return _dal.GetAllCategories();
        }
        //יצירת קטגוריה
        public BudgetCategories AddCategory(string categoryName, string? description)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("שם קטגוריה הוא שדה חובה");

            var cat = new BudgetCategories
            {
                CategoryName = categoryName.Trim(),
                Description = string.IsNullOrWhiteSpace(description)
                                ? null
                                : description.Trim()
            };
            int newId = _dal.AddCategory(cat.CategoryName, cat.Description);
            cat.CategoryId = newId;
            return cat;
        }
    }
}
