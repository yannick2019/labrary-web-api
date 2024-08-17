using FluentValidation;
using Library.API.Extensions;
using Library.API.Models;
using Library.API.Services.Interfaces;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        /// <summary>
        /// Gets a paginated list of books based on the provided parameters.
        /// </summary>
        /// <param name="bookParameters">The parameters for pagination and filtering of books.</param>
        /// <returns>A paginated list of books.</returns>
        [HttpGet(Name = "GetBooks")]
        public async Task<ActionResult<PaginatedList<Book>>> GetBooks([FromQuery] BookParameters bookParameters)
        {
            var books = await _bookService.GetPaginatedBooksAsync(bookParameters);
            books.CreatePaginationLinks(Url, "GetBooks", bookParameters);
            return Ok(books);
        }

        /// <summary>
        /// Gets a specific book by ID.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book with the specified ID.</returns>
        [HttpGet("{id}")]
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
        /// Creates a new book.
        /// </summary>
        /// <param name="book">The book to create.</param>
        /// <returns>The created book.</returns>
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            try
            {
                var createdBook = await _bookService.AddBookAsync(book);
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
        /// <returns>No content if the update was successful.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest("The book ID does not match the route ID.");
            }

            try
            {
                await _bookService.UpdateBookAsync(book);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Borrows a book by ID.
        /// </summary>
        /// <param name="id">The ID of the book to borrow.</param>
        /// <param name="userId">The ID of the user who borrow the book.</param>
        /// <returns>No content if the borrowing was successful.</returns>
        [HttpPost("{id}/borrow")]
        public async Task<ActionResult<Book>> BorrowBook(int id, [FromBody] int userId)
        {
            try
            {
                var book = await _bookService.BorrowBookAsync(id, userId);
                if (book.BorrowerId.HasValue && book.Borrower != null)
                {
                    var response = new BookBorrowResponse
                    {
                        BookId = book.Id,
                        Title = book.Title,
                        Author = book.Author,
                        BorrowerId = book.BorrowerId.Value,
                        BorrowerUsername = book.Borrower.Username
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
        /// Deletes a specific book.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
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