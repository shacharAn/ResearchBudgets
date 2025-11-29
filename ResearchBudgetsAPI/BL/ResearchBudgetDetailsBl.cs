using System;
using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.BL
{
    public class ResearchBudgetDetailsBl
    {
        private readonly ResearchBudgetDetailsDal _detailsDal;

        public ResearchBudgetDetailsBl()
        {
            _detailsDal = new ResearchBudgetDetailsDal();
        }

        public ResearchBudgetDetails GetResearchBudgetDetails(int researchId)
        {
            if (researchId <= 0)
                throw new ArgumentException("מספר מחקר אינו חוקי");

            return _detailsDal.GetResearchBudgetDetails(researchId);
        }
    }
}
