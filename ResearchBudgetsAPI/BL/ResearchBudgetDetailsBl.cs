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

            var details = _detailsDal.GetResearchBudgetDetails(researchId);
            if (details == null || details.ResearchId == 0)
                throw new Exception("לא נמצאו פרטי תקציב למחקר שביקשת");

            return details;
        }

    }
}
