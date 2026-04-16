using Modules.Users.Application.Session.OAuth.Google;

namespace Modules.Users.Application.UnitTests.Session.OAuth.Google;

internal sealed class LoginGoogleCommandTestBuilder
{
    private string _googleIdToken = "valid.google.jwt.token";

    private LoginGoogleCommandTestBuilder() { }

    public static LoginGoogleCommandTestBuilder ACommand() => new();

    public LoginGoogleCommandTestBuilder WithGoogleIdToken(string token)
    {
        _googleIdToken = token;
        return this;
    }

    public LoginGoogleCommand Build() => new(_googleIdToken);
}
