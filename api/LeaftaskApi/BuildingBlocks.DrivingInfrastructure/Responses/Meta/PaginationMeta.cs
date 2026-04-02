namespace BuildingBlocks.DrivingInfrastructure.Responses.Meta;

public class PaginationMeta
{
    public required int Limit { get; init; }
    public string? NextCursor { get; init; }
    public required bool HasMore { get; init; }
}
