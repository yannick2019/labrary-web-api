using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Library.API.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public new string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();
    }
}