using RuppinResearchBudget.Models;
using System.Data.SqlClient;
using System.Data;
using RuppinResearchBudget.DAL;

namespace RuppinResearchBudget.DAL
{
    public class BudgetCategoriesDal : DBServices
    {
        public List<BudgetCategories> GetAllCategories()
        {
            var list = new List<BudgetCategories>();

            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("sp_BudgetCategories_GetAll", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var cat = new BudgetCategories
                        {
                            CategoryId = Convert.ToInt32(rdr["CategoryId"]),
                            CategoryName = rdr["CategoryName"].ToString() ?? string.Empty,
                            Description = rdr["Description"] == DBNull.Value
                                           ? null
                                           : rdr["Description"].ToString()
                        };
                        list.Add(cat);
                    }
                }
            }
            return list;
        }
        public int AddCategory(string categoryName, string? description)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("sp_BudgetCategories_Add", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrWhiteSpace(description)
                        ? (object)DBNull.Value
                        : description);

                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    throw new Exception("יצירת הקטגוריה נכשלה במסד הנתונים");

                return Convert.ToInt32(result);
            }
        }
    }
}
