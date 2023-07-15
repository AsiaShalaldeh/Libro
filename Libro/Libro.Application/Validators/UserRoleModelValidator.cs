using FluentValidation;
using Libro.Domain.Enums;
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
                .Must(role => role.Equals(UserRole.Patron.ToString())
                || role.Equals(UserRole.Administrator.ToString())
                || role.Equals(UserRole.Librarian.ToString()))
                .WithMessage("Role must be either 'Patron', 'Admin', or 'Librarian'.");
        }
    }
}
