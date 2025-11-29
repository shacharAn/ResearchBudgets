using System;
using System.Collections.Generic;
using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.BL
{
    public class ResearchesBl
    {
        private readonly ResearchesDal _researchesDal;

        public ResearchesBl()
        {
            _researchesDal = new ResearchesDal();
        }

        public Researches CreateResearch(Researches research)
        {
            if (string.IsNullOrWhiteSpace(research.ResearchCode))
                throw new ArgumentException("קוד מחקר הוא שדה חובה");

            if (string.IsNullOrWhiteSpace(research.ResearchName))
                throw new ArgumentException("שם מחקר הוא שדה חובה");

            if (research.TotalBudget <= 0)
                throw new ArgumentException("תקציב המחקר חייב להיות גדול מאפס");

            if (research.EndDate.HasValue && research.EndDate < research.StartDate)
                throw new ArgumentException("תאריך סיום לא יכול להיות לפני תאריך התחלה");

            return _researchesDal.CreateResearch(research);
        }

        public Researches GetResearchById(int researchId)
        {
            if (researchId <= 0)
                throw new ArgumentException("מספר מחקר אינו חוקי");

            return _researchesDal.GetResearchById(researchId);
        }

        public List<Researches> GetResearchesByUser(string idNumber)
        {
            if (string.IsNullOrWhiteSpace(idNumber))
                throw new ArgumentException("תעודת זהות היא שדה חובה");

            return _researchesDal.GetResearchesByUser(idNumber);
        }
    }
}
