using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Library.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();
    }
}