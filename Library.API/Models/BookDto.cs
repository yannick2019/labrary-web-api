namespace Library.API.Models
{
    public class BookDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
    }
}