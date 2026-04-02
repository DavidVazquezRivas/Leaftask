using Microsoft.EntityFrameworkCore;
using Modules.Users.Application.Management.GetAll;
using Modules.Users.DrivenInfrastructure.Persistence;

namespace Modules.Users.DrivenInfrastructure.Queries;

public sealed class GetAllUsersQueryService(UserDbContext dbContext) : IGetAllUsersQueryService
{
    public async Task<IReadOnlyList<SimpleUserDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Users
            .AsNoTracking()
            .Select(user => new SimpleUserDto(
                user.Id,
                user.FirstName + " " + user.LastName,
                user.Email
            ))
            .ToListAsync(cancellationToken);
}
