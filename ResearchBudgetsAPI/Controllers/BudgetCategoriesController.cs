using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetCategoriesController : ControllerBase
    {
        private readonly BudgetCategoriesBl _bl = new BudgetCategoriesBl();

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var categories = _bl.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody] BudgetCategories dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = _bl.AddCategory(dto.CategoryName, dto.Description);
                return Ok(created);
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
