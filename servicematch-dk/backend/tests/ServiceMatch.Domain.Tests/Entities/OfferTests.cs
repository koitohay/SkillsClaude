using FluentAssertions;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Exceptions;

namespace ServiceMatch.Domain.Tests.Entities;

public class OfferTests
{
    private static Offer CreateOffer(decimal price = 500)
        => Offer.Create(Guid.NewGuid(), Guid.NewGuid(), price, null);

    [Fact]
    public void Create_StatusIsPending()
    {
        var offer = CreateOffer();
        offer.Status.Should().Be(OfferStatus.Pending);
        offer.Price.Amount.Should().Be(500);
    }

    [Fact]
    public void Accept_PendingOffer_StatusIsAccepted()
    {
        var offer = CreateOffer();
        offer.Accept();
        offer.Status.Should().Be(OfferStatus.Accepted);
    }

    [Fact]
    public void Decline_PendingOffer_StatusIsDeclined()
    {
        var offer = CreateOffer();
        offer.Decline();
        offer.Status.Should().Be(OfferStatus.Declined);
    }

    [Fact]
    public void Decline_AcceptedOffer_ThrowsDomainException()
    {
        var offer = CreateOffer();
        offer.Accept();
        var act = () => offer.Decline();
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddCounterOffer_CreatesNegotiationAndStatusIsCountered()
    {
        var offer = CreateOffer();
        var negotiation = offer.AddCounterOffer(NegotiationInitiator.Client, 450, "Can you do less?");

        offer.Status.Should().Be(OfferStatus.Countered);
        offer.Negotiations.Should().HaveCount(1);
        negotiation.ProposedPrice.Amount.Should().Be(450);
        negotiation.Status.Should().Be(NegotiationStatus.Pending);
    }

    [Fact]
    public void AddCounterOffer_BeyondMaxRounds_ThrowsDomainException()
    {
        var offer = CreateOffer();
        for (var i = 0; i < Offer.MaxNegotiationRounds; i++)
        {
            var initiator = i % 2 == 0 ? NegotiationInitiator.Client : NegotiationInitiator.Provider;
            offer.AddCounterOffer(initiator, 400 + i, null);
        }

        var act = () => offer.AddCounterOffer(NegotiationInitiator.Client, 300, null);
        act.Should().Throw<DomainException>().WithMessage("*Maximum*");
    }

    [Fact]
    public void AddCounterOffer_ToAcceptedOffer_ThrowsDomainException()
    {
        var offer = CreateOffer();
        offer.Accept();
        var act = () => offer.AddCounterOffer(NegotiationInitiator.Client, 400, null);
        act.Should().Throw<DomainException>();
    }
}
