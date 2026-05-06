using BuildingBlocks.Domain.Entities;
using Modules.Projects.Domain.Entities.Member;

namespace Modules.Projects.Domain.Entities.Owner;

public sealed class UserReadModel : Entity, IProjectOwner, IProjectMember
{
    private UserReadModel() { }

    public UserReadModel(Guid id, string firstName, string lastName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    public Guid Id { get; }
}
