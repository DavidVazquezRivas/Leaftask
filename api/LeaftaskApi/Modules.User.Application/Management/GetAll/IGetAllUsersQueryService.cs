using BuildingBlocks.Application.Queries;

namespace Modules.Users.Application.Management.GetAll;

public interface IGetAllUsersQueryService
{
    Task<PaginatedResult<SimpleUserDto>> GetAllAsync(
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken);
}
