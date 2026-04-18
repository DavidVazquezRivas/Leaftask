namespace BuildingBlocks.Application.Queries;

public sealed record PaginatedResult<T>(IReadOnlyCollection<T> Items, string? NextCursor, bool HasMore);
