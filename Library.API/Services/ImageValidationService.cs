using Library.API.Models;
using Library.API.Services.Interfaces;

namespace Library.API.Services
{
    public class ImageValidationService : IImageValidationService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ImageValidationResult ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ImageValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Aucun fichier n'a été fourni."
                };
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension))
            {
                return new ImageValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"The file type is not allowed. Allowed types : {string.Join(", ", _allowedExtensions)}"
                };
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return new ImageValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"File size exceeds {MaxFileSizeBytes / 1024 / 1024} MB."
                };
            }

            return new ImageValidationResult { IsValid = true };
        }
    }
}