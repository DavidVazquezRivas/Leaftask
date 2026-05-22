namespace Modules.Users.Application.Session;

public record SessionResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiresAt);
