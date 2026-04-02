using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Management.GetAll;

public sealed class GetAllUsersQuery : IPaginatedQuery<Result<IReadOnlyCollection<SimpleUserDto>>>
{
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public string[]? Sort { get; init; }
    public string? Search { get; init; }
}
