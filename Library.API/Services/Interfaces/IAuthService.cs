using Library.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Library.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
        Task<(IdentityResult Result, string Message)> RegisterUserAsync(RegisterRequest request);
        string GenerateJwtToken(User user, IList<string> roles);
        Task EnsureRolesCreatedAsync();
    }
}