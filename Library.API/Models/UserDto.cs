namespace Library.API.Models
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();
    }
}