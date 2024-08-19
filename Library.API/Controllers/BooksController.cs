using FluentValidation;
using Library.API.Extensions;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/books")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Gets all books.
        /// </summary>
        /// <returns>A list of all books.</returns>
        [HttpGet(Name = "GetAllBooks")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        /// <summary>
        /// Gets a paginated list of books based on the provided parameters.
        /// </summary>
        /// <param name="bookParameters">The parameters for pagination and filtering of books.</param>
        /// <returns>A paginated list of books.</returns>
        [HttpGet("paginated-list", Name = "GetBooks")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<PaginatedList<Book>>> GetBooks([FromQuery] BookParameters bookParameters)
        {
            var books = await _bookService.GetPaginatedBooksAsync(bookParameters);
            books.CreatePaginationLinks(Url, "GetBooks", bookParameters);
            return Ok(books);
        }

        /// <summary>
        /// Searches for books based on the provided search parameters.
        /// </summary>
        /// <param name="parameters">The parameters for searching and filtering books.</param>
        /// <returns>A paginated list of books that match the search criteria.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedList<BookDto>>> SearchBooks([FromQuery] BookSearchParameters parameters)
        {
            var books = await _bookService.SearchBooksAsync(parameters);
            return Ok(books);
        }

        /// <summary>
        /// Gets a specific book by ID.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book with the specified ID.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                //throw new KeyNotFoundException($"Book with ID {id} not found.");
                return NotFound($"Book with ID {id} Not found");
            }
            return Ok(book);
        }

        /// <summary>
        /// Handles the HTTP POST request to add a new book.
        /// </summary>
        /// <param name="book">The book object to be added.</param>
        /// <param name="image">The image file associated with the book.</param>
        /// <returns>The created book.</returns>
        /// <remarks>This action is restricted to users with the "Admin" role.</remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> PostBook(Book book, IFormFile image)
        {
            try
            {
                var createdBook = await _bookService.AddBookAsync(book, image);
                return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }

        }

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="book">The updated book.</param>
        /// <param name="image">The image file associated with the book.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <remarks>This action is restricted to users with the "Admin" role.</remarks>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, Book book, IFormFile image)
        {
            if (id != book.Id)
            {
                return BadRequest("The book ID does not match the route ID.");
            }

            try
            {
                await _bookService.UpdateBookAsync(book, image);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        /// <summary>
        /// Borrows a book by ID.
        /// </summary>
        /// <param name="id">The ID of the book to borrow.</param>
        /// <param name="userId">The ID of the user who borrow the book.</param>
        /// <returns>No content if the borrowing was successful.</returns>
        [HttpPost("{id}/borrow")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Book>> BorrowBook(int id, [FromBody] string userId)
        {
            try
            {
                var book = await _bookService.BorrowBookAsync(id, userId);
                if (!string.IsNullOrEmpty(book.BorrowerId) && book.Borrower != null)
                {
                    var response = new BookBorrowResponse
                    {
                        BookId = book.Id,
                        Title = book.Title,
                        Author = book.Author,
                        BorrowerId = book.BorrowerId,
                        BorrowerUsername = book.Borrower.UserName!
                    };
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Borrower information is missing.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns a borrowed book by ID.
        /// </summary>
        /// <param name="id">The ID of the book to return.</param>
        /// <returns>BookReturnResponse if the return was successful.</returns>
        [HttpPost("{id}/return")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<BookReturnResponse>> ReturnBook(int id)
        {
            try
            {
                var book = await _bookService.ReturnBookAsync(id);
                var response = new BookReturnResponse
                {
                    BookId = book.Id,
                    Title = book.Title,
                    IsReturned = true
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Adds a genre to a book.
        /// </summary>
        /// <param name="bookId">The ID of the book to which the genre will be added.</param>
        /// <param name="genreId">The ID of the genre to add to the book.</param>
        /// <returns>No content if the genre is successfully added to the book, or NotFound if the book or genre does not exist.</returns>
        /// <remarks>This action is restricted to users with the "Admin" role.</remarks>
        [HttpPost("{bookId}/genres/{genreId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddGenreToBook(int bookId, int genreId)
        {
            try
            {
                await _bookService.AddGenreToBookAsync(bookId, genreId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Removes a genre from a book.
        /// </summary>
        /// <param name="bookId">The ID of the book from which the genre will be removed.</param>
        /// <param name="genreId">The ID of the genre to remove from the book.</param>
        /// <returns>No content if the genre is successfully removed from the book, or NotFound if the book or genre does not exist.</returns>
        /// <remarks>This action is restricted to users with the "Admin" role.</remarks>
        [HttpDelete("{bookId}/genres/{genreId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveGenreFromBook(int bookId, int genreId)
        {
            try
            {
                await _bookService.RemoveGenreFromBookAsync(bookId, genreId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Deletes a specific book.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>NoContent.</returns>
        /// <remarks>This action is restricted to users with the "Admin" role.</remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

        private async Task<bool> BookExists(int id)
        {
            return await _bookService.GetBookByIdAsync(id) != null;
        }
    }
}