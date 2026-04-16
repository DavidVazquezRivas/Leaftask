using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Mvc;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Application.Management.Create;
using Modules.Organizations.DrivingInfrastructure.Models.Requests;

namespace Modules.Organizations.DrivingInfrastructure.Controllers;

[Route("api/v1/organizations")]
public class OrganizationManagementController : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateOrganizationCommand command = new(request.Name, request.Description, request.Website);

        Result<BasicOrganizationResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result, 201);
    }
}
