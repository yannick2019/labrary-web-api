namespace Library.API.Models
{
    public class BookSearchParameters
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int? PublicationYear { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int? GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;
    }
}