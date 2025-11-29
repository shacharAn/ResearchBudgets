using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int researchId, [FromForm] string uploadedById)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "לא הועלה קובץ" });

            try
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var storedFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var fullPath = Path.Combine(uploadsFolder, storedFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileModel = new Files
                {
                    ResearchId = researchId,
                    OriginalFileName = file.FileName,
                    StoredFileName = storedFileName,
                    RelativePath = Path.Combine("uploads", storedFileName),
                    ContentType = file.ContentType,
                    UploadedById = uploadedById,
                    UploadedAt = DateTime.Now
                };

                int newId = _filesBl.AddFile(fileModel);

                return Ok(new { FileId = newId, Path = fileModel.RelativePath });
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
