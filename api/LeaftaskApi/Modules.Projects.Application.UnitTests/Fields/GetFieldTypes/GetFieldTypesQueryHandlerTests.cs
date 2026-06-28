using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Fields.GetFieldTypes;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Fields.GetFieldTypes;

public class GetFieldTypesQueryHandlerTests
{
    private readonly GetFieldTypesQueryHandler _handler;
    private readonly IGetFieldTypesQueryService _queryServiceMock;

    public GetFieldTypesQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IGetFieldTypesQueryService>();
        _handler = new GetFieldTypesQueryHandler(_queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFieldTypes_When_QueryIsHandled()
    {
        // Arrange
        GetFieldTypesQuery query = new();

        IReadOnlyList<FieldTypeDto> fieldTypes =
        [
            new FieldTypeDto(Guid.NewGuid(), "Text", "A text field"),
            new FieldTypeDto(Guid.NewGuid(), "Number", "A numeric field")
        ];
        _queryServiceMock.GetFieldTypesAsync(Arg.Any<CancellationToken>()).Returns(fieldTypes);

        // Act
        Result<IReadOnlyList<FieldTypeDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(fieldTypes);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_NoFieldTypesExist()
    {
        // Arrange
        GetFieldTypesQuery query = new();

        _queryServiceMock.GetFieldTypesAsync(Arg.Any<CancellationToken>()).Returns([]);

        // Act
        Result<IReadOnlyList<FieldTypeDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
