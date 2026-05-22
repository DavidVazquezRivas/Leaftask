using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Application.Management.GetAll;
using Modules.Users.DrivenInfrastructure.Persistence;

namespace Modules.Users.DrivenInfrastructure.Queries;

public sealed class GetAllUsersQueryService(UserDbContext dbContext) : IGetAllUsersQueryService
{
    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<UserRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<UserRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", user => user.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["firstName"] = new("firstName", user => user.FirstName, value => value, value => (string)value),
            ["lastName"] = new("lastName", user => user.LastName, value => value, value => (string)value),
            ["email"] = new("email", user => user.Email, value => value, value => (string)value)
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["email:asc", "id:asc"];
    private const string IdSort = "id:asc";

    public async Task<PaginatedResult<SimpleUserDto>> GetAllAsync(
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken)
    {
        List<UserRow> users = await dbContext.Users
            .AsNoTracking()
            .Select(user => new UserRow(user.Id, user.FirstName, user.LastName, user.Email))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            users,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            user => new SimpleUserDto(user.Id, $"{user.FirstName} {user.LastName}", user.Email));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort) =>
        sort.Count == 0 || sort.Any(item => item.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? sort
            : [.. sort, IdSort];

    private sealed record UserRow(Guid Id, string FirstName, string LastName, string Email);
}
