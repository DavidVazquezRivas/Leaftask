using BuildingBlocks.Application.Queries;
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

        Result<PaginatedResult<SimpleUserDto>> result = await Sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        IReadOnlyList<SortMeta>? sortMeta = SortMetaParser.Parse(sort);
        PaginationMeta paginationMeta = new()
        {
            Limit = limit,
            NextCursor = result.Value.NextCursor,
            HasMore = result.Value.HasMore
        };

        return StatusCode(200, BuildSuccessResponse(result.Value.Items, sortMeta, paginationMeta));
    }
}
