using FluentValidation;
using Libro.Domain.Entities;

namespace Libro.Application.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.UserName)
                .NotNull().NotEmpty().WithMessage("Username is required.")
                .MaximumLength(20).WithMessage("Username must be less than or equal to 20 characters.");
        }
    }
}
