namespace Modules.Users.Application.Management.GetAll;

public interface IGetAllUsersQueryService
{
    Task<IReadOnlyList<SimpleUserDto>> GetAllAsync(CancellationToken cancellationToken);
}
