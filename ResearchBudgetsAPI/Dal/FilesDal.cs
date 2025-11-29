using System;
using System.Data;
using System.Data.SqlClient;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.DAL
{
    public class FilesDal : DBServices
    {

        public int AddFile(Files file)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spAddFile", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ResearchId", file.ResearchId);
                cmd.Parameters.AddWithValue("@OriginalFileName", file.OriginalFileName);
                cmd.Parameters.AddWithValue("@StoredFileName", file.StoredFileName);
                cmd.Parameters.AddWithValue("@RelativePath", file.RelativePath);
                cmd.Parameters.AddWithValue("@ContentType",
                    string.IsNullOrEmpty(file.ContentType) ? (object)DBNull.Value : file.ContentType);
                cmd.Parameters.AddWithValue("@UploadedById", file.UploadedById);

                object result = cmd.ExecuteScalar(); // NewFileId
                return Convert.ToInt32(result);
            }
        }
    }
}
