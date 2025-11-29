using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.DAL
{
    public class ResearchesDal : DBServices
    {
        public Researches? CreateResearch(Researches research)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spCreateResearch", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ResearchCode", research.ResearchCode);
                cmd.Parameters.AddWithValue("@ResearchName", research.ResearchName);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(research.Description) ? (object)DBNull.Value : research.Description);
                cmd.Parameters.AddWithValue("@TotalBudget", research.TotalBudget);
                cmd.Parameters.AddWithValue("@StartDate", research.StartDate);
                cmd.Parameters.AddWithValue("@EndDate",
                    research.EndDate.HasValue ? (object)research.EndDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IsUnderCenter", research.IsUnderCenter);
                cmd.Parameters.AddWithValue("@CenterId",
                    research.CenterId.HasValue ? (object)research.CenterId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@PrimaryResearcherId", research.PrimaryResearcherId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return MapResearch(reader);
                }
            }
        }

        public Researches? GetResearchById(int researchId)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spGetResearchById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ResearchId", researchId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return MapResearch(reader);
                }
            }
        }

        public List<Researches> GetResearchesByUser(string idNumber)
        {
            var list = new List<Researches>();

            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spGetResearchesByUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdNumber", idNumber);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapResearch(reader));
                    }
                }
            }

            return list;
        }

        private Researches MapResearch(SqlDataReader reader)
        {
            return new Researches
            {
                ResearchId = (int)reader["ResearchId"],
                ResearchCode = reader["ResearchCode"].ToString(),
                ResearchName = reader["ResearchName"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                TotalBudget = (decimal)reader["TotalBudget"],
                StartDate = (DateTime)reader["StartDate"],
                EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["EndDate"],
                IsUnderCenter = (bool)reader["IsUnderCenter"],
                CenterId = reader["CenterId"] == DBNull.Value ? (int?)null : (int)reader["CenterId"],
                PrimaryResearcherId = reader["PrimaryResearcherId"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }
    }
}
