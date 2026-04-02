using BuildingBlocks.Domain.Entities;

namespace Modules.Users.Domain.Entities;

public sealed class User : Entity
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }

    private User() { }

    private User(Guid id, string firstName, string lastName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public static User Create(Guid id, string firstName, string lastName, string email) => new(id, firstName, lastName, email);
}
