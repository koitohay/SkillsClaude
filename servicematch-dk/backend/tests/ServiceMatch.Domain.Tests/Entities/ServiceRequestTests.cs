using FluentAssertions;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Exceptions;

namespace ServiceMatch.Domain.Tests.Entities;

public class ServiceRequestTests
{
    private static readonly Guid ClientId = Guid.NewGuid();
    private static readonly DateOnly Tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
    private static readonly TimeOnly Noon = new(12, 0);

    private static ServiceRequest CreateRequest(int? categoryId = 1, string? description = null)
        => ServiceRequest.Create(ClientId, categoryId, description, Tomorrow, Noon, "Aarhus");

    [Fact]
    public void Create_WithCategory_StatusIsOpen()
    {
        var req = CreateRequest(categoryId: 1);
        req.Status.Should().Be(ServiceRequestStatus.Open);
        req.CategoryId.Should().Be(1);
    }

    [Fact]
    public void Create_WithFreeText_StatusIsOpen()
    {
        var req = CreateRequest(categoryId: null, description: "Need a plumber");
        req.Status.Should().Be(ServiceRequestStatus.Open);
        req.FreeTextDescription.Should().Be("Need a plumber");
    }

    [Fact]
    public void Create_WithNeitherCategoryNorDescription_ThrowsDomainException()
    {
        var act = () => ServiceRequest.Create(ClientId, null, null, Tomorrow, Noon, "Aarhus");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithPastDate_ThrowsDomainException()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var act = () => ServiceRequest.Create(ClientId, 1, null, yesterday, Noon, "Aarhus");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddOffer_FirstOffer_StatusBecomesOfferReceived()
    {
        var req = CreateRequest();
        req.AddOffer(Guid.NewGuid(), 500, null);
        req.Status.Should().Be(ServiceRequestStatus.OfferReceived);
        req.Offers.Should().HaveCount(1);
    }

    [Fact]
    public void AddOffer_SameProvider_ThrowsDomainException()
    {
        var req = CreateRequest();
        var providerId = Guid.NewGuid();
        req.AddOffer(providerId, 500, null);

        var act = () => req.AddOffer(providerId, 600, null);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddOffer_ToCancelledRequest_ThrowsDomainException()
    {
        var req = CreateRequest();
        req.Cancel();

        var act = () => req.AddOffer(Guid.NewGuid(), 500, null);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AcceptOffer_AcceptsOfferAndDeclinesOthers()
    {
        var req = CreateRequest();
        var offer1 = req.AddOffer(Guid.NewGuid(), 500, null);
        var offer2 = req.AddOffer(Guid.NewGuid(), 400, null);

        req.AcceptOffer(offer1.Id);

        req.Status.Should().Be(ServiceRequestStatus.Accepted);
        offer1.Status.Should().Be(OfferStatus.Accepted);
        offer2.Status.Should().Be(OfferStatus.Declined);
    }

    [Fact]
    public void AcceptOffer_WhenAlreadyAccepted_ThrowsDomainException()
    {
        var req = CreateRequest();
        var offer = req.AddOffer(Guid.NewGuid(), 500, null);
        req.AcceptOffer(offer.Id);

        var act = () => req.AcceptOffer(offer.Id);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_OpenRequest_StatusIsCancelled()
    {
        var req = CreateRequest();
        req.Cancel();
        req.Status.Should().Be(ServiceRequestStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AcceptedRequest_ThrowsDomainException()
    {
        var req = CreateRequest();
        var offer = req.AddOffer(Guid.NewGuid(), 500, null);
        req.AcceptOffer(offer.Id);

        var act = () => req.Cancel();
        act.Should().Throw<DomainException>();
    }
}
