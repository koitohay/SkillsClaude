using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Entities;

public class ServiceRequest
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public int? CategoryId { get; private set; }
    public string? FreeTextDescription { get; private set; }
    public DateOnly RequestedDate { get; private set; }
    public TimeOnly RequestedTime { get; private set; }
    public DanishCity City { get; private set; } = null!;
    public ServiceRequestStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<Offer> _offers = [];
    public IReadOnlyList<Offer> Offers => _offers.AsReadOnly();

    public ServiceCategory? Category { get; private set; }

    private ServiceRequest() { }

    public static ServiceRequest Create(
        Guid clientId,
        int? categoryId,
        string? freeTextDescription,
        DateOnly requestedDate,
        TimeOnly requestedTime,
        string city)
    {
        if (categoryId is null && string.IsNullOrWhiteSpace(freeTextDescription))
            throw new DomainException("A service request must have either a category or a description.");

        if (requestedDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("The requested date must be today or in the future.");

        return new ServiceRequest
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            CategoryId = categoryId,
            FreeTextDescription = freeTextDescription?.Trim(),
            RequestedDate = requestedDate,
            RequestedTime = requestedTime,
            City = DanishCity.Create(city),
            Status = ServiceRequestStatus.Open,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public Offer AddOffer(Guid providerId, decimal price, string? message)
    {
        if (Status == ServiceRequestStatus.Accepted || Status == ServiceRequestStatus.Cancelled)
            throw new DomainException("Cannot add an offer to a closed request.");

        if (_offers.Any(o => o.ServiceProviderId == providerId && o.Status != OfferStatus.Declined))
            throw new DomainException("This provider has already submitted an offer for this request.");

        var offer = Offer.Create(Id, providerId, price, message);
        _offers.Add(offer);

        if (Status == ServiceRequestStatus.Open)
            Status = ServiceRequestStatus.OfferReceived;

        return offer;
    }

    public void AcceptOffer(Guid offerId)
    {
        if (Status == ServiceRequestStatus.Accepted)
            throw new DomainException("This request already has an accepted offer.");

        if (Status == ServiceRequestStatus.Cancelled)
            throw new DomainException("Cannot accept an offer on a cancelled request.");

        var offer = _offers.FirstOrDefault(o => o.Id == offerId)
            ?? throw new DomainException("Offer not found on this request.");

        offer.Accept();
        Status = ServiceRequestStatus.Accepted;

        foreach (var other in _offers.Where(o => o.Id != offerId && o.Status == OfferStatus.Pending))
            other.Decline();
    }

    public void Cancel()
    {
        if (Status == ServiceRequestStatus.Accepted)
            throw new DomainException("An accepted request cannot be cancelled.");

        Status = ServiceRequestStatus.Cancelled;
    }
}
