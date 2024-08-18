namespace Library.API.Models
{
    public class BookBorrowResponse
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? BorrowerId { get; set; }
        public string BorrowerUsername { get; set; } = string.Empty;
    }
}