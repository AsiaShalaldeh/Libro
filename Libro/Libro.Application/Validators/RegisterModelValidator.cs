using FluentValidation;
using Libro.Domain.Models;

public class RegisterModelValidator : AbstractValidator<RegisterModel>
{
    public RegisterModelValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("User Name is required.")
            .Must(username => !string.IsNullOrWhiteSpace(username))
            .WithMessage("Username should not contain spaces.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }
}
