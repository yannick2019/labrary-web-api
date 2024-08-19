using Library.API.Services.Interfaces;

namespace Library.API.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly string _imageDirectory;

        public ImageStorageService(IWebHostEnvironment environment)
        {
            _imageDirectory = Path.Combine(environment.WebRootPath, "images", "books");
            Directory.CreateDirectory(_imageDirectory);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null!;

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_imageDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/books/" + fileName;
        }

        public Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return Task.CompletedTask;

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_imageDirectory, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }
    }
}