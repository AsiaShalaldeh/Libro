using FluentValidation;
using Libro.Domain.Dtos;

namespace Libro.Application.Validators
{
    public class PatronDtoValidator : AbstractValidator<PatronDto>
    {
        public PatronDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .MaximumLength(30).WithMessage("Name must not exceed 30 characters");

            RuleFor(dto => dto.Email)
            .EmailAddress().WithMessage("Invalid Email Format")
            .Must((dto, email) => !string.IsNullOrEmpty(dto.Name) || !string.IsNullOrEmpty(email))
            .WithMessage("Either Name or Email must be provided");
        }
    }

}
