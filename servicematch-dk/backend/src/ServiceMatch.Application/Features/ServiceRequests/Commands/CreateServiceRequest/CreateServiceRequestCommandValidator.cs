using FluentValidation;

namespace ServiceMatch.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public sealed class CreateServiceRequestCommandValidator : AbstractValidator<CreateServiceRequestCommand>
{
    public CreateServiceRequestCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.RequestedDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Requested date must be today or in the future.");
        RuleFor(x => x).Must(x => x.CategoryId.HasValue || !string.IsNullOrWhiteSpace(x.FreeTextDescription))
            .WithMessage("Either a category or a description must be provided.");
    }
}
