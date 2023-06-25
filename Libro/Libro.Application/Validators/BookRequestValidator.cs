using FluentValidation;
using Libro.Domain.Dtos;
using Libro.Domain.Enums;

namespace Libro.Application.Validators
{
    public class BookRequestValidator : AbstractValidator<BookRequest>
    {
        public BookRequestValidator(bool title = true, bool author = true, bool genre = true)
        {
            RuleFor(request => request.ISBN)
                .NotNull().NotEmpty().WithMessage("ISBN is required!");

            if (title)
            {
                RuleFor(request => request.Title)
                .NotNull().NotEmpty().WithMessage("Title is required!");
            }

            if (author)
            {
                RuleFor(request => request.AuthorId)
                .NotNull().NotEmpty().WithMessage("Author ID is required!");
            }

            RuleFor(request => request.AuthorId)
            .GreaterThan(0)
            .WithMessage("Author ID must be greater than 0");

            RuleFor(request => request.PublicationDate)
                .GreaterThanOrEqualTo(new DateTime(1900, 1, 1))
                .WithMessage("Publication Date must be greater than or equal to 1900-01-01");

            if (genre)
            {
                RuleFor(request => request.Genre)
                .NotNull().NotEmpty().WithMessage("Genre is required!");
            }

            RuleFor(request => request.Genre)
            .Must(genreValue => Enum.TryParse(typeof(Genre), genreValue, out _))
            .WithMessage("Invalid genre value");

            RuleFor(request => request.IsAvailable)
                .NotNull().NotEmpty().WithMessage("Availability is required!");
        }
    }

}
