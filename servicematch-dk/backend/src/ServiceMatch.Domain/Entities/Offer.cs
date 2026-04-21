using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Entities;

public class Offer
{
    public const int MaxNegotiationRounds = 10;

    public Guid Id { get; private set; }
    public Guid ServiceRequestId { get; private set; }
    public Guid ServiceProviderId { get; private set; }
    public Money Price { get; private set; } = null!;
    public string? Message { get; private set; }
    public OfferStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<Negotiation> _negotiations = [];
    public IReadOnlyList<Negotiation> Negotiations => _negotiations.AsReadOnly();

    private Offer() { }

    public static Offer Create(Guid serviceRequestId, Guid serviceProviderId, decimal price, string? message)
    {
        return new Offer
        {
            Id = Guid.NewGuid(),
            ServiceRequestId = serviceRequestId,
            ServiceProviderId = serviceProviderId,
            Price = Money.InDkk(price),
            Message = message?.Trim(),
            Status = OfferStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Accept()
    {
        if (Status != OfferStatus.Pending && Status != OfferStatus.Countered)
            throw new DomainException("Offer cannot be accepted in its current state.");
        Status = OfferStatus.Accepted;
    }

    public void Decline()
    {
        if (Status == OfferStatus.Accepted)
            throw new DomainException("An accepted offer cannot be declined.");
        Status = OfferStatus.Declined;
    }

    public Negotiation AddCounterOffer(NegotiationInitiator initiatedBy, decimal proposedPrice, string? message)
    {
        if (Status == OfferStatus.Accepted || Status == OfferStatus.Declined)
            throw new DomainException("Cannot counter a closed offer.");

        if (_negotiations.Count >= MaxNegotiationRounds)
            throw new DomainException($"Maximum of {MaxNegotiationRounds} negotiation rounds reached.");

        // Close any currently pending negotiation from the other side
        var pending = _negotiations.FirstOrDefault(n => n.Status == NegotiationStatus.Pending);
        pending?.Decline();

        var negotiation = Negotiation.Create(Id, initiatedBy, proposedPrice, message);
        _negotiations.Add(negotiation);
        Status = OfferStatus.Countered;
        return negotiation;
    }
}
