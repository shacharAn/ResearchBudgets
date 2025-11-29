using System;
using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.BL
{
    public class PaymentApprovalsBl
    {
        private readonly PaymentApprovalsDal _paymentApprovalsDal;

        public PaymentApprovalsBl()
        {
            _paymentApprovalsDal = new PaymentApprovalsDal();
        }

        public PaymentApprovals CreatePaymentApproval(int paymentRequestId, string approvedById,  string approvalRole,string status, string? comment)
                {
            if (paymentRequestId <= 0)
                throw new ArgumentException("מספר בקשת תשלום אינו חוקי");

            if (string.IsNullOrWhiteSpace(approvedById))
                throw new ArgumentException("מאשר התשלום הוא שדה חובה");

            if (string.IsNullOrWhiteSpace(approvalRole))
                throw new ArgumentException("תפקיד המאשר הוא שדה חובה");

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("סטטוס האישור הוא שדה חובה");

            var approval = _paymentApprovalsDal.CreatePaymentApproval(
                paymentRequestId, approvedById, approvalRole, status, comment);

            if (approval == null)
                throw new Exception("יצירת אישור התשלום נכשלה במסד הנתונים");

            return approval;
        }

    }
}
