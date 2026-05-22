namespace BuildingBlocks.DrivingInfrastructure.Responses.Meta;

public class ApiMeta
{
    public required string RequestId { get; init; }
    public required DateTime Timestamp { get; init; }
    public IEnumerable<SortMeta>? Sort { get; init; }
    public PaginationMeta? Pagination { get; init; }
}
