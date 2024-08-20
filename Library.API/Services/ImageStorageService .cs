using Library.API.Services.Interfaces;

namespace Library.API.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly string _imageDirectory;
        private readonly ILogger<ImageStorageService> _logger;

        public ImageStorageService(IWebHostEnvironment environment, ILogger<ImageStorageService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (environment == null)
            {
                _logger.LogError("IWebHostEnvironment is null");
                throw new ArgumentNullException(nameof(environment));
            }

            if (string.IsNullOrEmpty(environment.WebRootPath))
            {
                _logger.LogError("WebRootPath is null or empty");
                throw new ArgumentNullException(nameof(environment.WebRootPath));
            }

            _imageDirectory = Path.Combine(environment.WebRootPath, "images", "books");

            if (string.IsNullOrEmpty(_imageDirectory))
            {
                _logger.LogError("Image directory path is null or empty");
                throw new ArgumentNullException(nameof(_imageDirectory));
            }

            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }
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