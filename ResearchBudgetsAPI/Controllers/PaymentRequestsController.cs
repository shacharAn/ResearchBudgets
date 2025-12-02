using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentRequestsController : ControllerBase
    {
        private readonly PaymentRequestsBl _paymentRequestsBl = new PaymentRequestsBl();

        // POST: api/paymentrequests
        [HttpPost]
        public IActionResult Create([FromBody] PaymentRequests request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int newId = _paymentRequestsBl.CreatePaymentRequest(request);

                return Ok(new { PaymentRequestId = newId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/paymentrequests/by-research/{researchId}
        [HttpGet("by-research/{researchId:int}")]
        public IActionResult GetByResearch(int researchId)
        {
            try
            {
                List<PaymentRequestWithDetails> list = _paymentRequestsBl.GetPaymentRequestsByResearch(researchId);

                return Ok(list);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/paymentrequests/by-user/{requestedById}
        [HttpGet("by-user/{requestedById}")]
        public IActionResult GetByUser(string requestedById)
        {
            try
            {
                List<PaymentRequestWithDetails> list = _paymentRequestsBl.GetPaymentRequestsByUser(requestedById);

                return Ok(list);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // PUT: api/paymentrequests/{id}
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] PaymentRequests request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.PaymentRequestId)
                return BadRequest(new { message = "אי התאמה בין מזהה הבקשה לנתיב ה-URL" });

            try
            {
                _paymentRequestsBl.UpdatePaymentRequest(request);
                return Ok(new { message = "בקשת התשלום עודכנה בהצלחה" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/paymentrequests/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _paymentRequestsBl.DeletePaymentRequest(id);
                return Ok(new { message = "בקשת התשלום נמחקה בהצלחה" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
