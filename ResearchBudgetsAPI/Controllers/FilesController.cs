using System;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FilesBl _filesBl = new FilesBl();

        // POST: api/files
        [HttpPost]
        public IActionResult AddFile([FromBody] Files file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int newId = _filesBl.AddFile(file);

                return Ok(new { FileId = newId });
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
