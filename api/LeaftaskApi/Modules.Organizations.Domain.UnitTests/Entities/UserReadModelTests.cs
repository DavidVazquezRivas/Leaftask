using FluentAssertions;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using Xunit;

namespace Modules.Organizations.Domain.UnitTests.Entities;

#pragma warning disable CA1515
public class UserReadModelTests
#pragma warning restore CA1515
{
#pragma warning disable CA1822
    [Fact]
    public void Constructor_Should_InitializeUserReadModelWithCorrectData()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        const string firstName = "Clark";
        const string lastName = "Kent";
        const string email = "clark@dailyplanet.com";

        // Act
        UserReadModel userReadModel = UserReadModelTestBuilder.AUser()
            .WithId(id)
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithEmail(email)
            .Build();

        // Assert
        userReadModel.Id.Should().Be(id);
        userReadModel.FirstName.Should().Be(firstName);
        userReadModel.LastName.Should().Be(lastName);
        userReadModel.Email.Should().Be(email);
    }
#pragma warning restore CA1822
}
