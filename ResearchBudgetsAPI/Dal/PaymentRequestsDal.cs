using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.DAL
{
    public class PaymentRequestsDal : DBServices
    {

        public int CreatePaymentRequest(PaymentRequests request)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spCreatePaymentRequest", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ResearchId", request.ResearchId);
                cmd.Parameters.AddWithValue("@RequestedById", request.RequestedById);
                cmd.Parameters.AddWithValue("@CategoryId", request.CategoryId);
                cmd.Parameters.AddWithValue("@Amount", request.Amount);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(request.Description) ? (object)DBNull.Value : request.Description);
                cmd.Parameters.AddWithValue("@FileId",
                    request.FileId.HasValue ? (object)request.FileId.Value : DBNull.Value);

                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    throw new Exception("שגיאה ביצירת בקשת התשלום במסד הנתונים");

                return Convert.ToInt32(result);

            }
        }

        public List<PaymentRequestWithDetails> GetPaymentRequestsByResearch(int researchId)
        {
            var list = new List<PaymentRequestWithDetails>();

            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spGetPaymentRequestsByResearch", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ResearchId", researchId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PaymentRequestWithDetails
                        {
                            PaymentRequestId = (int)reader["PaymentRequestId"],
                            ResearchId = (int)reader["ResearchId"],
                            RequestedById = reader["RequestedById"].ToString(),
                            CategoryId = (int)reader["CategoryId"],
                            Amount = (decimal)reader["Amount"],
                            Currency = reader["Currency"].ToString(),
                            RequestDate = (DateTime)reader["RequestDate"],
                            Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                            FileId = reader["FileId"] == DBNull.Value ? (int?)null : (int)reader["FileId"],
                            Status = reader["Status"].ToString(),
                            CategoryName = reader["CategoryName"].ToString(),
                            RequestedByFirstName = reader["FirstName"].ToString(),
                            RequestedByLastName = reader["LastName"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public List<PaymentRequestWithDetails> GetPaymentRequestsByUser(string requestedById)
        {
            var list = new List<PaymentRequestWithDetails>();

            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spGetPaymentRequestsByUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestedById", requestedById);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PaymentRequestWithDetails
                        {
                            PaymentRequestId = (int)reader["PaymentRequestId"],
                            ResearchId = (int)reader["ResearchId"],
                            ResearchName = reader["ResearchName"].ToString(),
                            CategoryId = (int)reader["CategoryId"],
                            CategoryName = reader["CategoryName"].ToString(),
                            Amount = (decimal)reader["Amount"],
                            Currency = reader["Currency"].ToString(),
                            RequestDate = (DateTime)reader["RequestDate"],
                            Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                            FileId = reader["FileId"] == DBNull.Value ? (int?)null : (int)reader["FileId"],
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}
