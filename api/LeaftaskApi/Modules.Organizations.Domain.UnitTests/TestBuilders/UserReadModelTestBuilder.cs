using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.UnitTests.TestBuilders;

#pragma warning disable CA1515
public sealed class UserReadModelTestBuilder
#pragma warning restore CA1515
{
    private string _email = "bruce@wayneenterprises.com";
    private string _firstName = "Bruce";
    private Guid _id = Guid.NewGuid();
    private string _lastName = "Wayne";

    private UserReadModelTestBuilder() { }

    public static UserReadModelTestBuilder AUser() => new();

    public UserReadModelTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserReadModelTestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserReadModelTestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserReadModelTestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserReadModel Build() => new(_id, _firstName, _lastName, _email);
}
