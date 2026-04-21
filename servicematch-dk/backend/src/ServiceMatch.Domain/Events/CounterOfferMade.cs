using ServiceMatch.Domain.Enums;

namespace ServiceMatch.Domain.Events;

public sealed record CounterOfferMade(Guid OfferId, Guid NegotiationId, NegotiationInitiator InitiatedBy);
