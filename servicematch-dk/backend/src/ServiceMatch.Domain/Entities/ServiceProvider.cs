using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Entities;

public class ServiceProvider
{
    public Guid Id { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public string ContactName { get; private set; } = string.Empty;
    public EmailAddress Email { get; private set; } = null!;
    public DanishPhoneNumber PhoneNumber { get; private set; } = null!;
    public string Address { get; private set; } = string.Empty;
    public DanishCity City { get; private set; } = null!;
    public CvrNumber CvrNumber { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsVerified { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<ProviderCategory> _categories = [];
    public IReadOnlyList<ProviderCategory> Categories => _categories.AsReadOnly();

    private readonly List<ProviderService> _services = [];
    public IReadOnlyList<ProviderService> Services => _services.AsReadOnly();

    private ServiceProvider() { }

    public static ServiceProvider Create(
        string companyName,
        string contactName,
        string email,
        string phoneNumber,
        string address,
        string city,
        string cvrNumber,
        string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(companyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactName);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new ServiceProvider
        {
            Id = Guid.NewGuid(),
            CompanyName = companyName.Trim(),
            ContactName = contactName.Trim(),
            Email = EmailAddress.Create(email),
            PhoneNumber = DanishPhoneNumber.Create(phoneNumber),
            Address = address.Trim(),
            City = DanishCity.Create(city),
            CvrNumber = ValueObjects.CvrNumber.Create(cvrNumber),
            PasswordHash = passwordHash,
            IsVerified = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void AssignCategories(IEnumerable<int> categoryIds)
    {
        _categories.Clear();
        foreach (var id in categoryIds.Distinct())
            _categories.Add(new ProviderCategory(Id, id));
    }

    public void Update(string companyName, string contactName, string email, string phoneNumber, string address, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(companyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactName);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        CompanyName = companyName.Trim();
        ContactName = contactName.Trim();
        Email = EmailAddress.Create(email);
        PhoneNumber = DanishPhoneNumber.Create(phoneNumber);
        Address = address.Trim();
        City = DanishCity.Create(city);
    }

    public ProviderService AddService(int? categoryId, string name, string description, decimal basePrice)
    {
        var service = ProviderService.Create(Id, categoryId, name, description, basePrice);
        _services.Add(service);
        return service;
    }

    public void UpdateService(Guid serviceId, string name, string description, decimal basePrice)
    {
        var service = _services.FirstOrDefault(s => s.Id == serviceId)
            ?? throw new InvalidOperationException($"Service {serviceId} not found.");
        service.Update(name, description, basePrice);
    }

    public void RemoveService(Guid serviceId)
    {
        var service = _services.FirstOrDefault(s => s.Id == serviceId)
            ?? throw new InvalidOperationException($"Service {serviceId} not found.");
        _services.Remove(service);
    }
}
