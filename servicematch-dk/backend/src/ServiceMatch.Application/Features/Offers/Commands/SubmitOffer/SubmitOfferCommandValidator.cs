using FluentValidation;

namespace ServiceMatch.Application.Features.Offers.Commands.SubmitOffer;

public sealed class SubmitOfferCommandValidator : AbstractValidator<SubmitOfferCommand>
{
    public SubmitOfferCommandValidator()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
