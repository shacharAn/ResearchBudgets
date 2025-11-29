using System;
using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.BL
{
    public class FilesBl  
    {
        private readonly FilesDal _filesDal;

        public FilesBl()
        {
            _filesDal = new FilesDal();
        }

        public int AddFile(Files file)
        {
            if (file.ResearchId <= 0)
                throw new ArgumentException("מספר מחקר אינו חוקי");

            if (string.IsNullOrWhiteSpace(file.OriginalFileName))
                throw new ArgumentException("שם הקובץ המקורי הוא שדה חובה");

            if (string.IsNullOrWhiteSpace(file.StoredFileName))
                throw new ArgumentException("שם הקובץ הפנימי (StoredFileName) הוא שדה חובה");

            if (string.IsNullOrWhiteSpace(file.RelativePath))
                throw new ArgumentException("נתיב הקובץ הוא שדה חובה");

            if (string.IsNullOrWhiteSpace(file.UploadedById))
                throw new ArgumentException("מזהה המעלה הוא שדה חובה");

            return _filesDal.AddFile(file);
        }
    }
}
