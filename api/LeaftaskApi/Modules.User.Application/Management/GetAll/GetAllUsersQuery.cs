using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Management.GetAll;

public sealed class GetAllUsersQuery : IPaginatedQuery<Result<PaginatedResult<SimpleUserDto>>>
{
    public string? Search { get; init; }
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
