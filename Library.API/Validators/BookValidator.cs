using FluentValidation;
using Library.API.Models;

namespace Library.API.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(book => book.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("The title cannot exceed 100 characters");

            RuleFor(book => book.Author)
                .NotEmpty().WithMessage("Author is required")
                .MaximumLength(50).WithMessage("Author's name cannot exceed 50 characters");

            RuleFor(book => book.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .Matches(@"^(?:\d{10}|\d{13})$").WithMessage("ISBN must be between 10 and 13 characters long");

            RuleFor(book => book.PublicationYear)
                .InclusiveBetween(1000, 9999).WithMessage("Year of publication must be between 1000 and 9999");

            RuleFor(book => book)
                .Must(book => !string.IsNullOrEmpty(book.Title) && !book.Title.Equals(book.Author, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Book title cannot be identical to author's name");
        }
    }
}