using Library.API.Models;

namespace Library.API.Services.Interfaces
{
    public interface IImageValidationService
    {
        ImageValidationResult ValidateImage(IFormFile file);
    }
}