using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentApprovalsController : ControllerBase
    {
        private readonly PaymentApprovalsBl _approvalsBl = new PaymentApprovalsBl();

        public class CreatePaymentApprovalRequest
        {
            [Required]
            public int PaymentRequestId { get; set; }
            [Required]
            public string ApprovedById { get; set; }
            [Required]
            public string ApprovalRole { get; set; }
            [Required]
            public string Status { get; set; }
            [Required]
            public string? Comment { get; set; }
        }

        // POST: api/paymentapprovals
        [HttpPost]
        public IActionResult Create([FromBody] CreatePaymentApprovalRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                PaymentApprovals result = _approvalsBl.CreatePaymentApproval(
                    request.PaymentRequestId,
                    request.ApprovedById,
                    request.ApprovalRole,
                    request.Status,
                    request.Comment
                );

                return Ok(result);
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
