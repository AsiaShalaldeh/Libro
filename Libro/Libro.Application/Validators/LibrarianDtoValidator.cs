using FluentValidation;
using Libro.Domain.Dtos;

namespace Libro.Application.Validators
{
    public class LibrarianDtoValidator : AbstractValidator<LibrarianDto>
    {
        public LibrarianDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().NotEmpty().WithMessage("Librarian Name is Required!")
                .MaximumLength(30).WithMessage("Name must not exceed 30 characters");
        }
    }
}
