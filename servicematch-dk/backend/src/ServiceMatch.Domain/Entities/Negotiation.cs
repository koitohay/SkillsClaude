using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Entities;

public class Negotiation
{
    public Guid Id { get; private set; }
    public Guid OfferId { get; private set; }
    public NegotiationInitiator InitiatedBy { get; private set; }
    public Money ProposedPrice { get; private set; } = null!;
    public string? Message { get; private set; }
    public NegotiationStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Negotiation() { }

    public static Negotiation Create(Guid offerId, NegotiationInitiator initiatedBy, decimal proposedPrice, string? message)
    {
        return new Negotiation
        {
            Id = Guid.NewGuid(),
            OfferId = offerId,
            InitiatedBy = initiatedBy,
            ProposedPrice = Money.InDkk(proposedPrice),
            Message = message?.Trim(),
            Status = NegotiationStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Accept()
    {
        if (Status != NegotiationStatus.Pending)
            throw new DomainException("Only pending negotiations can be accepted.");
        Status = NegotiationStatus.Accepted;
    }

    public void Decline()
    {
        if (Status != NegotiationStatus.Pending)
            throw new DomainException("Only pending negotiations can be declined.");
        Status = NegotiationStatus.Declined;
    }
}
