using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public EmailAddress Email { get; private set; } = null!;
    public DanishPhoneNumber PhoneNumber { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private Client() { }

    public static Client Create(string fullName, string email, string phoneNumber, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new Client
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = EmailAddress.Create(email),
            PhoneNumber = DanishPhoneNumber.Create(phoneNumber),
            PasswordHash = passwordHash,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Update(string fullName, string email, string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        FullName = fullName.Trim();
        Email = EmailAddress.Create(email);
        PhoneNumber = DanishPhoneNumber.Create(phoneNumber);
    }
}
