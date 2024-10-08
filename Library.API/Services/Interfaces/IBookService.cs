using Library.API.Models;

namespace Library.API.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book, IFormFile image);
        Task UpdateBookAsync(Book book, IFormFile image);
        Task DeleteBookAsync(int id);
        Task<Book> BorrowBookAsync(int bookId, string userId);
        Task<Book> ReturnBookAsync(int bookId);
        Task<PaginatedList<Book>> GetPaginatedBooksAsync(BookParameters bookParameters);
        Task<PaginatedList<BookDto>> SearchBooksAsync(BookSearchParameters parameters);
        Task AddGenreToBookAsync(int bookId, int genreId);
        Task RemoveGenreFromBookAsync(int bookId, int genreId);
    }
}