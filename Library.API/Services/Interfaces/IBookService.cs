using Library.API.Models;

namespace Library.API.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<Book> BorrowBookAsync(int bookId, string userId);
        Task<Book> ReturnBookAsync(int bookId);
        Task<PaginatedList<Book>> GetPaginatedBooksAsync(BookParameters bookParameters);
    }
}