namespace Library.API.Models
{
    public class BookReturnResponse
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsReturned { get; set; }
    }
}