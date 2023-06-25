using FluentValidation;
using Libro.Domain.Dtos;

namespace Libro.Application.Validators
{
    public class AuthorDtoValidator : AbstractValidator<AuthorDto>
    {
        public AuthorDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotNull().NotEmpty().WithMessage("Author Name is required!")
                .MaximumLength(30).WithMessage("Author name must not exceed 30 characters");
        }
    }

}
