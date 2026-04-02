using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Responses;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Error = BuildingBlocks.Domain.Result.Error;

namespace BuildingBlocks.DrivingInfrastructure.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiBaseController : ControllerBase
{
    private ISender? _sender;

    // On demand dependency injection (lazy loading)
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult<T>(
        Result<T> result,
        int successStatusCode = StatusCodes.Status200OK,
        IReadOnlyList<SortMeta>? sort = null,
        PaginationMeta? pagination = null)
    {
        if (result.IsSuccess)
        {
            ApiResponse<T> response = BuildSuccessResponse(result.Value, sort, pagination);
            return StatusCode(successStatusCode, response);
        }

        return HandleFailure(result.Error);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleFailure(result.Error);
    }

    private ObjectResult HandleFailure(Error error)
    {
        ApiMeta meta = new()
        {
            Timestamp = DateTime.UtcNow,
            RequestId = HttpContext.TraceIdentifier
        };
        ErrorDetails errorDetails = new(error.Code, error.Description);

        ApiResponse response = ApiResponse.Failure(errorDetails, meta);

        return StatusCode(error.StatusCode, response);
    }

    private ApiResponse<T> BuildSuccessResponse<T>(T data, IReadOnlyList<SortMeta>? sort = null, PaginationMeta? pagination = null)
    {
        ApiMeta meta = new()
        {
            Timestamp = DateTime.UtcNow,
            RequestId = HttpContext.TraceIdentifier,
            Sort = sort,
            Pagination = pagination
        };

        return ApiResponse<T>.Success(data, meta);
    }
}
