/*using Microsoft.AspNetCore.Mvc;

namespace Ai_LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetImages : ControllerBase
    {
        [HttpGet("file/{filename}")]
        public IActionResult GetFileByName(string filename)
        {
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Search all subdirectories for the file
            var filePath = Directory.GetFiles(uploadsRoot, filename, SearchOption.AllDirectories).FirstOrDefault();

            if (filePath == null || !System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var mimeType = GetMimeType(filename);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, mimeType);
        }

        private string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}
*/