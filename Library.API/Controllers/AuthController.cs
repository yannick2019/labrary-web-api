using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns a token if successful.
        /// </summary>
        /// <param name="request">The authentication request containing user credentials.</param>
        /// <returns>An IActionResult containing the authentication response or an unauthorized status.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            var response = await _authService.AuthenticateAsync(request);
            if (response == null)
            {
                return Unauthorized();
            }
            return Ok(response);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration request containing user details.</param>
        /// <returns>An IActionResult containing the registration result and message.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (result, message) = await _authService.RegisterUserAsync(request);

            if (result.Succeeded)
            {
                return Ok(new { message });
            }

            return BadRequest(result.Errors);
        }
    }
}