using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Result;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Events;
using Modules.Users.Domain.Factories;

namespace Modules.Users.Domain.Entities;

public sealed class User : Entity
{
    private readonly List<RefreshToken> _refreshTokens = [];
    private User() { }

    private User(Guid id, string firstName, string lastName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static User Create(string firstName, string lastName, string email)
    {
        User user = new(Guid.NewGuid(), firstName, lastName, email);

        user.Raise(new UserCreatedDomainEvent(user.Id, user.FirstName, user.LastName, user.Email));

        return user;
    }

    public RefreshToken AddRefreshToken(IRefreshTokenFactory factory)
    {
        RefreshToken refreshToken = factory.CreateForUser(Id);
        _refreshTokens.Add(refreshToken);

        return refreshToken;
    }

    public Result<RefreshToken> RotateRefreshToken(string currentRefreshToken, IRefreshTokenFactory factory)
    {
        RefreshToken currentToken = _refreshTokens.FirstOrDefault(rt => rt.Token == currentRefreshToken);

        if (currentToken is null || !currentToken.IsValid)
        {
            return Result.Failure<RefreshToken>(UserErrors.InvalidRefreshToken);
        }

        RefreshToken newToken = AddRefreshToken(factory);

        currentToken.Revoke();

        return Result.Success(newToken);
    }

    public void RevokeAllRefreshTokens()
    {
        IEnumerable<RefreshToken> activeTokens = _refreshTokens.Where(rt => rt.IsValid);

        foreach (RefreshToken refreshToken in activeTokens)
        {
            refreshToken.Revoke();
        }
    }
}
