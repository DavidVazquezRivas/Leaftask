using FluentAssertions;
using Modules.Organizations.Domain.Errors;
using Xunit;

namespace Modules.Organizations.Domain.UnitTests.Errors;

#pragma warning disable CA1515
public class OrganizationErrorsTests
#pragma warning restore CA1515
{
#pragma warning disable CA1822
    [Fact]
    public void OrganizationNotFound_Should_HaveExpectedValues()
    {
        // Assert
        OrganizationErrors.OrganizationNotFound.Code.Should().Be("Organization.NotFound");
        OrganizationErrors.OrganizationNotFound.Description.Should().Be("The specified organization was not found.");
        OrganizationErrors.OrganizationNotFound.StatusCode.Should().Be(404);
    }
#pragma warning restore CA1822
}
