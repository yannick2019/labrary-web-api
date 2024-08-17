using FluentValidation;
using Library.API.Data;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryContext _context;
        private readonly IValidator<Book> _validator;

        public BookService(LibraryContext context, IValidator<Book> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            var validationResult = await _validator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task UpdateBookAsync(Book book)
        {
            var validationResult = await _validator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Book> BorrowBookAsync(int bookId, int userId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
                //return NotFound("Book not found");
            }

            if (book.BorrowerId.HasValue)
            {
                throw new InvalidOperationException("This book is already borrowed.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
                //return NotFound("User not found.");
            }

            book.BorrowerId = userId;
            book.Borrower = user;

            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<Book> ReturnBookAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
            }

            if (!book.BorrowerId.HasValue)
            {
                throw new InvalidOperationException("This book is not currently borrowed.");
            }

            book.BorrowerId = null;
            book.Borrower = null;

            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<PaginatedList<Book>> GetPaginatedBooksAsync(BookParameters bookParameters)
        {
            IQueryable<Book> books = _context.Books;

            // Appliquer le tri
            switch (bookParameters.SortBy.ToLower())
            {
                case "author":
                    books = bookParameters.SortDescending
                        ? books.OrderByDescending(b => b.Author)
                        : books.OrderBy(b => b.Author);
                    break;
                case "publicationyear":
                    books = bookParameters.SortDescending
                        ? books.OrderByDescending(b => b.PublicationYear)
                        : books.OrderBy(b => b.PublicationYear);
                    break;
                default: // Par défaut, trier par titre
                    books = bookParameters.SortDescending
                        ? books.OrderByDescending(b => b.Title)
                        : books.OrderBy(b => b.Title);
                    break;
            }

            var count = await books.CountAsync();
            var items = await books.Skip((bookParameters.PageNumber - 1) * bookParameters.PageSize)
                                   .Take(bookParameters.PageSize)
                                   .ToListAsync();

            return new PaginatedList<Book>(items, count, bookParameters.PageNumber, bookParameters.PageSize);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}