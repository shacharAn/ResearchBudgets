using System;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResearchBudgetDetailsController : ControllerBase
    {
        private readonly ResearchBudgetDetailsBl _budgetBl = new ResearchBudgetDetailsBl();

        // GET: api/ResearchBudgetDetails/{researchId}
        [HttpGet("{researchId:int}")]
        public IActionResult GetResearchBudgetDetails(int researchId)
        {
            try
            {
                ResearchBudgetDetails details = _budgetBl.GetResearchBudgetDetails(researchId);

                if (details == null)
                    return NotFound(new { message = "לא נמצא מידע תקציבי למחקר המבוקש" });

                return Ok(details);
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
