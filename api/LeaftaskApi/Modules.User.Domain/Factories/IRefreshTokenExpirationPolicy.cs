namespace Modules.Users.Domain.Factories;

public interface IRefreshTokenExpirationPolicy
{
    TimeSpan ExpirationDuration { get; }
}
