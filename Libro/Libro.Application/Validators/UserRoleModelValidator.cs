using FluentValidation;
using Libro.Domain.Models;

namespace Libro.Application.Validators
{
    public class UserRoleModelValidator : AbstractValidator<UserRoleModel>
    {
        public UserRoleModelValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User Id is required.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => role == "Patron" || role == "Admin" || role == "Librarian")
                .WithMessage("Role must be either 'Patron', 'Admin', or 'Librarian'.");
        }
    }
}
