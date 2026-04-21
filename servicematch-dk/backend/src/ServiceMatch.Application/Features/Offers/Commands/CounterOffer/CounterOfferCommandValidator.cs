using FluentValidation;

namespace ServiceMatch.Application.Features.Offers.Commands.CounterOffer;

public class CounterOfferCommandValidator : AbstractValidator<CounterOfferCommand>
{
    public CounterOfferCommandValidator()
    {
        RuleFor(x => x.ProposedPrice).GreaterThan(0).WithMessage("Proposed price must be greater than 0.");
        RuleFor(x => x.ServiceRequestId).NotEmpty();
        RuleFor(x => x.OfferId).NotEmpty();
    }
}
