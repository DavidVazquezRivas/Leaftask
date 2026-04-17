using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using Xunit;

namespace Modules.Organizations.Domain.UnitTests.Entities;

#pragma warning disable CA1515
public class OrganizationInvitationTests
#pragma warning restore CA1515
{
#pragma warning disable CA1822
    [Fact]
    public void Create_Should_InitializeInvitationAsPending()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        // Act
        OrganizationInvitation invitation = OrganizationInvitationTestBuilder.AnInvitation()
            .WithOrganizationId(organizationId)
            .WithUserId(userId)
            .Build();

        // Assert
        invitation.Id.Should().NotBe(Guid.Empty);
        invitation.OrganizationId.Should().Be(organizationId);
        invitation.UserId.Should().Be(userId);
        invitation.Status.Should().Be(InvitationStatus.Pending);
        invitation.RespondedAt.Should().BeNull();
        invitation.AbandonedAt.Should().BeNull();
    }

    [Fact]
    public void Accept_Should_ReturnSuccess_When_InvitationIsPending()
    {
        // Arrange
        OrganizationInvitation invitation = OrganizationInvitationTestBuilder.AnInvitation().Build();

        // Act
        Result result = invitation.Accept();

        // Assert
        result.IsSuccess.Should().BeTrue();
        invitation.Status.Should().Be(InvitationStatus.Accepted);
        invitation.RespondedAt.Should().NotBeNull();
    }

    [Fact]
    public void Accept_Should_ReturnFailure_When_InvitationIsNotPending()
    {
        // Arrange
        OrganizationInvitation invitation = OrganizationInvitationTestBuilder.AnInvitation().Build();
        invitation.Accept();

        // Act
        Result result = invitation.Accept();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.InvalidInvitationStatus);
    }

    [Fact]
    public void Reject_Should_ReturnSuccess_When_InvitationIsPending()
    {
        // Arrange
        OrganizationInvitation invitation = OrganizationInvitationTestBuilder.AnInvitation().Build();

        // Act
        Result result = invitation.Reject();

        // Assert
        result.IsSuccess.Should().BeTrue();
        invitation.Status.Should().Be(InvitationStatus.Rejected);
        invitation.RespondedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_Should_ReturnSuccess_When_InvitationIsPending()
    {
        // Arrange
        OrganizationInvitation invitation = OrganizationInvitationTestBuilder.AnInvitation().Build();

        // Act
        Result result = invitation.Cancel();

        // Assert
        result.IsSuccess.Should().BeTrue();
        invitation.Status.Should().Be(InvitationStatus.Canceled);
        invitation.RespondedAt.Should().NotBeNull();
    }

    [Fact]
    public void Abandon_Should_ReturnSuccess_When_InvitationIsAccepted()
    {
        // Arrange
        OrganizationInvitation invitation = OrganizationInvitationTestBuilder.AnInvitation().Build();
        invitation.Accept();

        // Act
        Result result = invitation.Abandon();

        // Assert
        result.IsSuccess.Should().BeTrue();
        invitation.Status.Should().Be(InvitationStatus.Abandoned);
        invitation.AbandonedAt.Should().NotBeNull();
    }

    [Fact]
    public void Abandon_Should_ReturnFailure_When_InvitationIsNotAccepted()
    {
        // Arrange
        OrganizationInvitation invitation = OrganizationInvitationTestBuilder.AnInvitation().Build();

        // Act
        Result result = invitation.Abandon();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.InvalidInvitationStatus);
    }
#pragma warning restore CA1822
}
