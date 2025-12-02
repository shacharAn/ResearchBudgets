using System;
using System.Data;
using System.Data.SqlClient;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.DAL
{
    public class PaymentApprovalsDal : DBServices
    {
        public PaymentApprovals CreatePaymentApproval(
            int paymentRequestId,
            string approvedById,
            string approvalRole,
            string status,
            string? comment)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spCreatePaymentApproval", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PaymentRequestId", paymentRequestId);
                cmd.Parameters.AddWithValue("@ApprovedById", approvedById);
                cmd.Parameters.AddWithValue("@ApprovalRole", approvalRole);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Comment",
                    string.IsNullOrEmpty(comment) ? (object)DBNull.Value : comment);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new PaymentApprovals
                    {
                        ApprovalId = (int)reader["ApprovalId"],
                        PaymentRequestId = (int)reader["PaymentRequestId"],
                        ApprovedById = reader["ApprovedById"].ToString(),
                        ApprovalRole = reader["ApprovalRole"].ToString(),
                        Status = reader["Status"].ToString(),
                        ApprovalDate = (DateTime)reader["ApprovalDate"],
                        Comment = reader["Comment"] == DBNull.Value ? null : reader["Comment"].ToString()
                    };
                }
            }
        }
    }
}
