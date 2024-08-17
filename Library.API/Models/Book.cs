using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Library.API.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required")]
        [StringLength(50, ErrorMessage = "Author's name cannot exceed 50 characters")]
        public string Author { get; set; } = string.Empty;

        // ISBN: International Standard Book Number.
        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 13 characters long")]
        [RegularExpression(@"^(?:\d{10}|\d{13})$", ErrorMessage = "ISBN must be 10 or 13 digits long")]
        public string ISBN { get; set; } = string.Empty;

        [Range(1000, 9999, ErrorMessage = "Year of publication must be between 1000 and 9999")]
        public int PublicationYear { get; set; }

        public int? BorrowerId { get; set; }

        [JsonIgnore]
        public User? Borrower { get; set; }
    }
}