using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;
using Microsoft.AspNetCore.Hosting;

namespace RuppinResearchBudget.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FilesBl _filesBl = new FilesBl();
        private readonly IWebHostEnvironment _env;

        public FilesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // DTO שמייצג את הטופס
        public class UploadFileRequest
        {
            public int ResearchId { get; set; }
            public string UploadedById { get; set; } = string.Empty;
            public IFormFile File { get; set; } = default!;
        }

        // POST: api/files/upload
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public IActionResult Upload([FromForm] UploadFileRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "לא הועלה קובץ" });

            try
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                string uploadsFolder = Path.Combine(webRoot, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string extension = Path.GetExtension(request.File.FileName);
                string storedFileName = $"{Guid.NewGuid()}{extension}";
                string physicalPath = Path.Combine(uploadsFolder, storedFileName);

                using (var stream = System.IO.File.Create(physicalPath))
                {
                    request.File.CopyTo(stream);
                }

                string relativePath = $"uploads/{storedFileName}";

                // 2. שורה בטבלת Files
                var fileModel = new Files
                {
                    ResearchId = request.ResearchId,
                    OriginalFileName = request.File.FileName,
                    StoredFileName = storedFileName,
                    RelativePath = relativePath,
                    ContentType = request.File.ContentType,
                    UploadedById = request.UploadedById,
                    UploadedAt = DateTime.Now
                };

                int fileId = _filesBl.AddFile(fileModel);
                return Ok(new { FileId = fileId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
