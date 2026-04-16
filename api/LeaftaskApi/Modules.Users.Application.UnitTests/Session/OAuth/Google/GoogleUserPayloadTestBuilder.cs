using Modules.Users.Application.Session.OAuth.Google;

namespace Modules.Users.Application.UnitTests.Session.OAuth.Google;

internal sealed class GoogleUserPayloadTestBuilder
{
    private string _email = "clark.kent@dailyplanet.com";
    private string _firstName = "Clark";
    private string _lastName = "Kent";

    private GoogleUserPayloadTestBuilder() { }

    public static GoogleUserPayloadTestBuilder APayload() => new();

    public GoogleUserPayloadTestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public GoogleUserPayloadTestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public GoogleUserPayloadTestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public GoogleUserPayload Build() => new(_email, _firstName, _lastName);
}
