using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.GetProject;

namespace Modules.Projects.DrivingInfrastructure.Tools;

public class ProjectManagementAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetProjectDetails")]
    [Description(
        "Retrieves the detailed information of a specific project by its ID. Useful to get the full context, abbreviation, privacy level, and core settings of a project.")]
    public async Task<string> GetProjectAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        GetProjectQuery query = new(projectId);

        Result<ProjectResponse> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetProjectAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }
}
