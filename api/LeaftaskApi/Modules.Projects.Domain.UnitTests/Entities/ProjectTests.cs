using FluentAssertions;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.UnitTests.TestBuilders;

namespace Modules.Projects.Domain.UnitTests.Entities;

#pragma warning disable CA1515
public class ProjectTests
#pragma warning restore CA1515
{
#pragma warning disable CA1822
    [Fact]
    public void Create_Should_InitializeProjectWithCorrectData_When_OwnedByUser()
    {
        // Arrange
        const string name = "My Project";
        const string abbreviation = "MYP";
        const ProjectPrivacy privacy = ProjectPrivacy.Private;
        Guid userId = Guid.NewGuid();

        // Act
        Project project = ProjectTestBuilder.AProject()
            .WithName(name)
            .WithAbbreviation(abbreviation)
            .WithPrivacy(privacy)
            .OwnedByUser(userId)
            .Build();

        // Assert
        project.Id.Should().NotBe(Guid.Empty);
        project.Name.Should().Be(name);
        project.Abbreviation.Should().Be(abbreviation);
        project.Privacy.Should().Be(privacy);
        project.OwnerType.Should().Be(OwnerType.User);
        project.Owner.Id.Should().Be(userId);
        project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_Should_InitializeProjectWithCorrectData_When_OwnedByOrganization()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();

        // Act
        Project project = ProjectTestBuilder.AProject()
            .OwnedByOrganization(organizationId)
            .Build();

        // Assert
        project.OwnerType.Should().Be(OwnerType.Organization);
        project.Owner.Id.Should().Be(organizationId);
    }
    [Fact]
    public void Update_Should_ChangeOnlyProvidedFields()
    {
        // Arrange
        Project project = ProjectTestBuilder.AProject()
            .WithName("Original Name")
            .WithAbbreviation("ORG")
            .WithPrivacy(ProjectPrivacy.Public)
            .Build();

        // Act
        project.Update(name: "Updated Name", abbreviation: null, privacy: ProjectPrivacy.Private);

        // Assert
        project.Name.Should().Be("Updated Name");
        project.Abbreviation.Should().Be("ORG");
        project.Privacy.Should().Be(ProjectPrivacy.Private);
    }

    [Fact]
    public void Update_Should_NotChangeAnyField_When_AllArgumentsAreNull()
    {
        // Arrange
        Project project = ProjectTestBuilder.AProject()
            .WithName("Original Name")
            .WithAbbreviation("ORG")
            .WithPrivacy(ProjectPrivacy.Public)
            .Build();

        // Act
        project.Update(null, null, null);

        // Assert
        project.Name.Should().Be("Original Name");
        project.Abbreviation.Should().Be("ORG");
        project.Privacy.Should().Be(ProjectPrivacy.Public);
    }
#pragma warning restore CA1822
}
