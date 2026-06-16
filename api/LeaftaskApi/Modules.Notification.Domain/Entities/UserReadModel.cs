using BuildingBlocks.Domain.Entities;

namespace Modules.Notification.Domain.Entities;

public sealed class UserReadModel : Entity
{
    private UserReadModel() { }

    public UserReadModel(Guid id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
}
