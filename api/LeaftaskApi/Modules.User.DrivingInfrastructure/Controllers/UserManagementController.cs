using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Management.GetAll;

namespace Modules.Users.DrivingInfrastructure.Controllers;

[Route("api/v1/users")]
public class UserManagementController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] IReadOnlyCollection<string>? sort = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        GetAllUsersQuery query = new()
        {
            Limit = limit,
            Cursor = cursor,
            Sort = sort ?? [],
            Search = search
        };

        Result<IReadOnlyCollection<SimpleUserDto>> result = await Sender.Send(query, cancellationToken);

        // TODO metadata not implemented yet
        IReadOnlyList<SortMeta>? sortMeta = SortMetaParser.Parse(sort);
        PaginationMeta paginationMeta = new()
        {
            Limit = limit,
            NextCursor = cursor,
            HasMore = true
        };

        return HandleResult(result, 200, sortMeta, paginationMeta);
    }
}
