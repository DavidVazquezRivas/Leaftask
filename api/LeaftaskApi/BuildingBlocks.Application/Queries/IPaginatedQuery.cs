namespace BuildingBlocks.Application.Queries;

public interface IPaginatedQuery<out TResponse> : IQuery<TResponse>
{
    int Limit { get; }
    string? Cursor { get; }
    IReadOnlyCollection<string> Sort { get; }
}
