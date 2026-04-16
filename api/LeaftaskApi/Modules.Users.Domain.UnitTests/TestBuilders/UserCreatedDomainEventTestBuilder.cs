using Modules.Users.Domain.Events;

namespace Modules.Users.Domain.UnitTests.TestBuilders;

#pragma warning disable CA1515 // Consider making public types internal
public sealed class UserCreatedDomainEventTestBuilder
#pragma warning restore CA1515 // Consider making public types internal
{
    private string _email = "bruce@wayneenterprises.com";
    private string _firstName = "Bruce";
    private string _lastName = "Wayne";
    private Guid _userId = Guid.NewGuid();

    private UserCreatedDomainEventTestBuilder() { }

    public static UserCreatedDomainEventTestBuilder AnEvent() => new();

    public UserCreatedDomainEventTestBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public UserCreatedDomainEventTestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserCreatedDomainEventTestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserCreatedDomainEventTestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserCreatedDomainEvent Build() =>
        new(_userId, _firstName, _lastName, _email);
}
