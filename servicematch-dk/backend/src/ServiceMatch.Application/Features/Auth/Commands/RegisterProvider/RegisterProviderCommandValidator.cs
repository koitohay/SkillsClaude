using FluentValidation;

namespace ServiceMatch.Application.Features.Auth.Commands.RegisterProvider;

public sealed class RegisterProviderCommandValidator : AbstractValidator<RegisterProviderCommand>
{
    public RegisterProviderCommandValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.CvrNumber).NotEmpty().Length(8).Matches(@"^\d{8}$");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.CategoryIds).NotEmpty().WithMessage("At least one service category is required.");
    }
}
