

namespace Ai_LibraryApi.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;
        public string WebRootPath => _environment.WebRootPath!;

        public FileService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));

            if (string.IsNullOrEmpty(_environment.WebRootPath))
            {
                _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (!Directory.Exists(_environment.WebRootPath))
                {
                    Directory.CreateDirectory(_environment.WebRootPath);
                }
            }
        }
        public async Task<string> SaveFileAsync(IFormFile file, string folder, string? oldFileName = null)
        {
            if (!string.IsNullOrEmpty(oldFileName))
            {
                DeleteImage(Path.Combine("uploads", folder, oldFileName));
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName; // ✅ just the filename
        }


        public async Task<string> SaveImageAsync(IFormFile file, string imageType, string? oldImagePath = null)
        {
            ValidateImageFile(file);
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                DeleteImage(Path.Combine("uploads", imageType, oldImagePath));
            }

            var fileName = await SaveImage(file, imageType);
            return fileName; // ✅ just the filename
        }

        private async Task<string> SaveImage(IFormFile image, string imageType)
        {
            if (string.IsNullOrEmpty(_environment.WebRootPath))
            {
                throw new InvalidOperationException("Web root path is not configured");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", imageType);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return fileName;
        }
        public void DeleteImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            var filePath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void ValidateImageFile(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png" };
            var maxSize = 5 * 1024 * 1024; // 5MB
            if (!allowedTypes.Contains(file.ContentType))
                throw new InvalidOperationException("Only JPEG and PNG images are allowed");
            if (file.Length > maxSize)
                throw new InvalidOperationException("Image size exceeds 5MB limit");
        }
    }
}