using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Management.GetAll;

public sealed class GetAllUsersQueryHandler(IGetAllUsersQueryService service)
    : IQueryHandler<GetAllUsersQuery, Result<PaginatedResult<SimpleUserDto>>>
{
    public async Task<Result<PaginatedResult<SimpleUserDto>>> Handle(GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<SimpleUserDto> users = await service.GetAllAsync(
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);

        return Result.Success(users);
    }
}
