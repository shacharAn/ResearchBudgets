using System;
using System.Collections.Generic;
using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.BL
{
    public class PaymentRequestsBl 
    {
        private readonly PaymentRequestsDal _paymentRequestsDal;

        public PaymentRequestsBl()
        {
            _paymentRequestsDal = new PaymentRequestsDal();
        }

        public int CreatePaymentRequest(PaymentRequests request)
        {
            if (request.ResearchId <= 0)
                throw new ArgumentException("מספר מחקר אינו חוקי");

            if (string.IsNullOrWhiteSpace(request.RequestedById))
                throw new ArgumentException("מבקש התשלום הוא שדה חובה");

            if (request.CategoryId <= 0)
                throw new ArgumentException("קטגוריית תקציב היא שדה חובה");

            if (request.Amount <= 0)
                throw new ArgumentException("סכום התשלום חייב להיות גדול מאפס");

            return _paymentRequestsDal.CreatePaymentRequest(request);
        }

        public List<PaymentRequestWithDetails> GetPaymentRequestsByResearch(int researchId)
        {
            if (researchId <= 0)
                throw new ArgumentException("מספר מחקר אינו חוקי");

            return _paymentRequestsDal.GetPaymentRequestsByResearch(researchId);
        }

        public List<PaymentRequestWithDetails> GetPaymentRequestsByUser(string requestedById)
        {
            if (string.IsNullOrWhiteSpace(requestedById))
                throw new ArgumentException("תעודת זהות היא שדה חובה");

            return _paymentRequestsDal.GetPaymentRequestsByUser(requestedById);
        }
    }
}
