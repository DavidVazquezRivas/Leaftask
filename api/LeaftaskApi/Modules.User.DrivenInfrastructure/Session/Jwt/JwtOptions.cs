namespace Modules.Users.DrivenInfrastructure.Session.Jwt;

public class JwtOptions
{
    public const string SectionName = "Modules:Users:Session:Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInDays { get; init; }
}
