using Modules.Organizations.Application.Users.Create;

namespace Modules.Organizations.Application.UnitTests.TestBuilders;

internal sealed class CreateUserReadModelOnUserCreatedCommandTestBuilder
{
    private string _email = "diana@themiscira.org";
    private string _firstName = "Diana";
    private string _lastName = "Prince";
    private Guid _userId = Guid.NewGuid();

    private CreateUserReadModelOnUserCreatedCommandTestBuilder() { }

    public static CreateUserReadModelOnUserCreatedCommandTestBuilder ACommand() => new();

    public CreateUserReadModelOnUserCreatedCommandTestBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public CreateUserReadModelOnUserCreatedCommandTestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public CreateUserReadModelOnUserCreatedCommandTestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public CreateUserReadModelOnUserCreatedCommandTestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CreateUserReadModelOnUserCreatedCommand Build() =>
        new(_userId, _firstName, _lastName, _email);
}
