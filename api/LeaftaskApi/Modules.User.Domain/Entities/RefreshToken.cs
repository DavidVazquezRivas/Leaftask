using System.Security.Cryptography;

namespace Modules.Users.Domain.Entities;

public sealed class RefreshToken
{
    private RefreshToken() { }
    internal bool IsValid => !IsRevoked && !IsExpired;
    internal bool IsRevoked => RevokedAt.HasValue;
    internal bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public Guid Id { get; set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private init; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public static RefreshToken Create(Guid userId, TimeSpan duration) =>
        new()
        {
            UserId = userId,
            Token = GenerateToken(),
            ExpiresAt = DateTime.UtcNow.Add(duration),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = null
        };

    internal RefreshToken Revoke()
    {
        RevokedAt = DateTime.UtcNow;
        return this;
    }

    private static string GenerateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
