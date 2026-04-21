namespace ServiceMatch.Domain.Events;

public sealed record OfferAccepted(Guid OfferId, Guid ServiceRequestId, Guid ClientId, Guid ServiceProviderId);
