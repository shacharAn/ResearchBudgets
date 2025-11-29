using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResearchesController : ControllerBase
    {
        private readonly ResearchesBl _researchesBl = new ResearchesBl();

        // POST: api/researches
        [HttpPost]
        public IActionResult CreateResearch([FromBody] Researches research)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                Researches created = _researchesBl.CreateResearch(research);
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

        // GET: api/researches/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                Researches research = _researchesBl.GetResearchById(id);

                if (research == null)
                    return NotFound(new { message = "המחקר לא נמצא" });

                return Ok(research);
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

        // GET: api/researches/by-user/{idNumber}
        [HttpGet("by-user/{idNumber}")]
        public IActionResult GetByUser(string idNumber)
        {
            try
            {
                List<Researches> researches = _researchesBl.GetResearchesByUser(idNumber);
                return Ok(researches);
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
