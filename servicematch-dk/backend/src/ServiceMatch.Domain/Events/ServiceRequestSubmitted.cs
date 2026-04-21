namespace ServiceMatch.Domain.Events;

public sealed record ServiceRequestSubmitted(Guid ServiceRequestId, Guid ClientId);
