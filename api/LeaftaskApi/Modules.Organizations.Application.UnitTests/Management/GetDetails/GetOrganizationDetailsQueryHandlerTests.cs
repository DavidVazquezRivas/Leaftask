using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Application.Management.GetDetails;
using Modules.Organizations.Application.UnitTests.TestBuilders;
using Modules.Organizations.Domain.Errors;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Management.GetDetails;

public class GetOrganizationDetailsQueryHandlerTests
{
    private readonly GetOrganizationDetailsQueryHandler _handler;
    private readonly IGetOrganizationDetailsQueryService _serviceMock;

    public GetOrganizationDetailsQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetOrganizationDetailsQueryService>();
        _handler = new GetOrganizationDetailsQueryHandler(_serviceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationExists()
    {
        // Arrange
        GetOrganizationDetailsQuery query = GetOrganizationDetailsQueryTestBuilder.AQuery().Build();
        BasicOrganizationResponse response = BasicOrganizationResponseTestBuilder.AResponse()
            .WithId(query.Id)
            .Build();

        _serviceMock.GetOrganizationDetailsAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        Result<BasicOrganizationResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationDoesNotExist()
    {
        // Arrange
        GetOrganizationDetailsQuery query = GetOrganizationDetailsQueryTestBuilder.AQuery().Build();

        _serviceMock.GetOrganizationDetailsAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns((BasicOrganizationResponse?)null);

        // Act
        Result<BasicOrganizationResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationNotFound);
    }
}
