using Modules.Projects.Application.Management.Create;
using Modules.Projects.Domain.Entities;

namespace Modules.Projects.Application.UnitTests.TestBuilders;

internal sealed class CreateProjectCommandTestBuilder
{
    private string _name = "Test Project";
    private string _abbreviation = "TST";
    private ProjectPrivacy _privacyLevel = ProjectPrivacy.Public;
    private Guid? _organizationId;

    private CreateProjectCommandTestBuilder() { }

    public static CreateProjectCommandTestBuilder ACommand() => new();

    public CreateProjectCommandTestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CreateProjectCommandTestBuilder WithAbbreviation(string abbreviation)
    {
        _abbreviation = abbreviation;
        return this;
    }

    public CreateProjectCommandTestBuilder WithPrivacyLevel(ProjectPrivacy privacyLevel)
    {
        _privacyLevel = privacyLevel;
        return this;
    }

    public CreateProjectCommandTestBuilder WithOrganizationId(Guid organizationId)
    {
        _organizationId = organizationId;
        return this;
    }

    public CreateProjectCommand Build() => new(_name, _abbreviation, _privacyLevel, _organizationId);
}
