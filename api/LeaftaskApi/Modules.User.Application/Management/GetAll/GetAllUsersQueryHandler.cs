using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Management.GetAll;

public sealed class GetAllUsersQueryHandler(IGetAllUsersQueryService service)
    : IQueryHandler<GetAllUsersQuery, Result<IReadOnlyCollection<SimpleUserDto>>>
{
    public async Task<Result<IReadOnlyCollection<SimpleUserDto>>> Handle(GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        // TODO pagination
        IReadOnlyCollection<SimpleUserDto> users = await service.GetAllAsync(cancellationToken);
        return Result.Success(users);
    }
}
