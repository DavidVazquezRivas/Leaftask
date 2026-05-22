using Modules.Users.Application.Management.GetAll;

namespace Modules.Users.Application.UnitTests.Management.GetAll;

internal sealed class SimpleUserDtoTestBuilder
{
    private readonly Guid _id = Guid.NewGuid();
    private string _email = "bruce@wayneenterprises.com";
    private string _firstName = "Bruce";
    private string _lastName = "Wayne";

    private SimpleUserDtoTestBuilder() { }

    public static SimpleUserDtoTestBuilder ADto() => new();

    public SimpleUserDtoTestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public SimpleUserDtoTestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public SimpleUserDtoTestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public SimpleUserDto Build() => new(_id, $"{_firstName} {_lastName}", _email);
}
