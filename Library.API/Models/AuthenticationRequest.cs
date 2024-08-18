using System.ComponentModel.DataAnnotations;

namespace Library.API.Models
{
    public class AuthenticationRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}