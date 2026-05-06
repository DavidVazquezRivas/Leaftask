using Modules.Projects.Domain.Entities;

namespace Modules.Projects.Application.Management.Create;

public static class ProjectPrivacyLevels
{
    public static readonly Guid PublicId = Guid.Parse("0A7DEB24-203D-4C4B-B4F4-9A5E6C67B101");
    public static readonly Guid RestrictedId = Guid.Parse("3F4CB8A2-93CD-46AB-8C20-95A0F9CFA102");
    public static readonly Guid PrivateId = Guid.Parse("E2B3A173-44D7-4B2D-8D19-55C3A61BB103");

    public static bool TryMap(Guid id, out ProjectPrivacy privacy)
    {
        if (id == PublicId)
        {
            privacy = ProjectPrivacy.Public;
            return true;
        }

        if (id == RestrictedId)
        {
            privacy = ProjectPrivacy.Restricted;
            return true;
        }

        if (id == PrivateId)
        {
            privacy = ProjectPrivacy.Private;
            return true;
        }

        privacy = default;
        return false;
    }

    public static PrivacyLevelDto ToDto(ProjectPrivacy privacy) =>
        privacy switch
        {
            ProjectPrivacy.Public => new(PublicId, "Public"),
            ProjectPrivacy.Restricted => new(RestrictedId, "Restricted"),
            ProjectPrivacy.Private => new(PrivateId, "Private"),
            _ => throw new ArgumentOutOfRangeException(nameof(privacy), privacy, null)
        };
}
