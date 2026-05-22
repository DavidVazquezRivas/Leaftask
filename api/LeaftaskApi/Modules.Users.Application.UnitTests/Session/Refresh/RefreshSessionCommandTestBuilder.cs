using Modules.Users.Application.Session.Refresh;

namespace Modules.Users.Application.UnitTests.Session.Refresh;

internal sealed class RefreshSessionCommandTestBuilder
{
    private string _refreshToken = "default-valid-refresh-token";

    private RefreshSessionCommandTestBuilder() { }

    public static RefreshSessionCommandTestBuilder ACommand() => new();

    public RefreshSessionCommandTestBuilder WithRefreshToken(string refreshToken)
    {
        _refreshToken = refreshToken;
        return this;
    }

    public RefreshSessionCommand Build() => new(_refreshToken);
}
