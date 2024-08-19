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
        private readonly IImageStorageService _imageStorageService;
        private readonly IImageValidationService _imageValidationService;

        public BookService(
            LibraryContext context,
            IValidator<Book> validator,
            IImageStorageService imageStorageService,
            IImageValidationService imageValidationService)
        {
            _context = context;
            _validator = validator;
            _imageStorageService = imageStorageService;
            _imageValidationService = imageValidationService;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book> AddBookAsync(Book book, IFormFile image)
        {
            var validationResult = await _validator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (image != null)
            {
                var imageValidationResult = _imageValidationService.ValidateImage(image);
                if (!imageValidationResult.IsValid)
                {
                    throw new ArgumentException(imageValidationResult.ErrorMessage);
                }
                book.ImageUrl = await _imageStorageService.UploadImageAsync(image);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task UpdateBookAsync(Book book, IFormFile image)
        {
            var validationResult = await _validator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (image != null)
            {
                var imageValidationResult = _imageValidationService.ValidateImage(image);
                if (!imageValidationResult.IsValid)
                {
                    throw new ArgumentException(imageValidationResult.ErrorMessage);
                }
                await _imageStorageService.DeleteImageAsync(book.ImageUrl!);
                book.ImageUrl = await _imageStorageService.UploadImageAsync(image);
            }

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Book> BorrowBookAsync(int bookId, string userId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
                //return NotFound("Book not found");
            }

            if (string.IsNullOrEmpty(book.BorrowerId))
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

            if (string.IsNullOrEmpty(book.BorrowerId))
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
                default:
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

        public async Task<PaginatedList<BookDto>> SearchBooksAsync(BookSearchParameters parameters)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Title))
            {
                query = query.Where(b => EF.Functions.Like(b.Title, $"%{parameters.Title}%"));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Author))
            {
                query = query.Where(b => EF.Functions.Like(b.Author, $"%{parameters.Author}%"));
            }

            if (!string.IsNullOrWhiteSpace(parameters.ISBN))
            {
                query = query.Where(b => EF.Functions.Like(b.ISBN, parameters.ISBN));
            }

            if (parameters.PublicationYear.HasValue)
            {
                query = query.Where(b => b.PublicationYear == parameters.PublicationYear.Value);
            }

            if (parameters.GenreId.HasValue)
            {
                query = query.Where(b => b.Genres.Any(g => g.Id == parameters.GenreId.Value));
            }

            if (!string.IsNullOrWhiteSpace(parameters.GenreName))
            {
                query = query.Where(b => b.Genres.Any(g => EF.Functions.Like(g.Name, $"%{parameters.GenreName}%")));
            }

            var count = await query.CountAsync();
            var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                                   .Take(parameters.PageSize)
                                   .ToListAsync();

            var bookDtos = items.Select(MapToDto).ToList();

            return new PaginatedList<BookDto>(bookDtos, count, parameters.PageNumber, parameters.PageSize);
        }

        public async Task AddGenreToBookAsync(int bookId, int genreId)
        {
            var book = await _context.Books.FindAsync(bookId);
            var genre = await _context.Genres.FindAsync(genreId);

            if (book == null || genre == null)
            {
                throw new KeyNotFoundException("Book or Genre not found");
            }

            book.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveGenreFromBookAsync(int bookId, int genreId)
        {
            var book = await _context.Books.Include(b => b.Genres).FirstOrDefaultAsync(b => b.Id == bookId);
            var genre = await _context.Genres.FindAsync(genreId);

            if (book == null || genre == null)
            {
                throw new KeyNotFoundException("Book or Genre not found");
            }

            book.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                await _imageStorageService.DeleteImageAsync(book.ImageUrl!);
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }


        private BookDto MapToDto(Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublicationYear = book.PublicationYear,
                Genres = book.Genres.Select(g => g.Name).ToList()
            };
        }
    }
}