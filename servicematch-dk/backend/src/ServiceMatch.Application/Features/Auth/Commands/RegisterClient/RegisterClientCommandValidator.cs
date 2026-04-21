using FluentValidation;

namespace ServiceMatch.Application.Features.Auth.Commands.RegisterClient;

public sealed class RegisterClientCommandValidator : AbstractValidator<RegisterClientCommand>
{
    public RegisterClientCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
