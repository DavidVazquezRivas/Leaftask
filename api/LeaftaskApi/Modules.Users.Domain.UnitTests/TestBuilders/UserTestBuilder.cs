using Modules.Users.Domain.Entities;

namespace Modules.Users.Domain.UnitTests.TestBuilders;

#pragma warning disable CA1515
public sealed class UserTestBuilder
#pragma warning restore CA1515
{
    private string _email = "bruce@wayneenterprises.com";
    private string _firstName = "Bruce";
    private string _lastName = "Wayne";

    private UserTestBuilder() { }

    public static UserTestBuilder AUser() => new();

    public UserTestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserTestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserTestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public User Build() => User.Create(_firstName, _lastName, _email);
}
