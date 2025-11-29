using System;
using System.Data;
using System.Data.SqlClient;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.DAL
{
    public class ResearchBudgetDetailsDal : DBServices
    {
        public ResearchBudgetDetails GetResearchBudgetDetails(int researchId)
        {
            var result = new ResearchBudgetDetails();

            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spGetResearchBudgetDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ResearchId", researchId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    //  פרטי המחקר
                    if (reader.Read())
                    {
                        result.ResearchId = (int)reader["ResearchId"];
                        result.ResearchName = reader["ResearchName"].ToString();
                        result.ResearchCode = reader["ResearchCode"].ToString();
                        result.TotalBudget = (decimal)reader["TotalBudget"];
                        result.StartDate = (DateTime)reader["StartDate"];
                        result.EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["EndDate"];
                        result.CreatedAt = (DateTime)reader["CreatedAt"];
                    }
                    else
                    {
                        throw new Exception("לא נמצאו נתוני מחקר עבור המזהה שנשלח");
                    }

                    // סך כל ההוצאות המאושרות
                    if (reader.NextResult() && reader.Read() && reader["TotalApprovedExpenses"] != DBNull.Value)
                    {
                        result.TotalApprovedExpenses = (decimal)reader["TotalApprovedExpenses"];
                    }

                    // הוצאות לפי קטגוריה
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            result.ExpensesByCategory.Add(new BudgetCategoryTotal
                            {
                                CategoryId = (int)reader["CategoryId"],
                                CategoryName = reader["CategoryName"].ToString(),
                                TotalByCategory = (decimal)reader["TotalByCategory"]
                            });
                        }
                    }

                    // פירוט מלא של כל הבקשות
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            result.Requests.Add(new PaymentRequestDetailsRow
                            {
                                PaymentRequestId = (int)reader["PaymentRequestId"],
                                Amount = (decimal)reader["Amount"],
                                RequestDate = (DateTime)reader["RequestDate"],
                                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                                Status = reader["Status"].ToString(),
                                FileId = reader["FileId"] == DBNull.Value ? (int?)null : (int)reader["FileId"],
                                CategoryName = reader["CategoryName"].ToString(),
                                RequestedBy = reader["RequestedBy"].ToString()
                            });
                        }
                    }

                    // יתרת תקציב
                    if (reader.NextResult() && reader.Read())
                    {
                        result.RemainingBudget = (decimal)reader["RemainingBudget"];
                    }
                }
            }
            return result;
        }
    }
}
