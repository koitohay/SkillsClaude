namespace ServiceMatch.Application.DTOs;

public record AuthTokenDto(string Token, string Role, Guid UserId);
