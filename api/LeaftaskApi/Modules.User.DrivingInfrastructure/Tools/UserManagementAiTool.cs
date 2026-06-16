using System.ComponentModel;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Users.Application.Management.GetAll;

namespace Modules.Users.DrivingInfrastructure.Tools;

public class UserManagementAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("SearchUsers")]
    [Description("Searches and retrieves a list of system users. Useful for finding team members to assign tasks.")]
    public async Task<string> SearchUsersAsync(
        [Description("Search term (e.g., full name, email).")]
        string? search = null,
        [Description("Max results. Default is 10.")]
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        GetAllUsersQuery query = new()
        {
            Limit = limit,
            Search = search
        };

        Result<PaginatedResult<SimpleUserDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(SearchUsersAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value.Items);
    }
}
