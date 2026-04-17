using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Errors;

namespace Modules.Organizations.Application.Management.GetDetails;

public sealed class GetOrganizationDetailsQueryHandler(IGetOrganizationDetailsQueryService service)
    : IQueryHandler<GetOrganizationDetailsQuery, Result<BasicOrganizationResponse>>
{
    public async Task<Result<BasicOrganizationResponse>> Handle(GetOrganizationDetailsQuery request,
        CancellationToken cancellationToken)
    {
        BasicOrganizationResponse? response =
            await service.GetOrganizationDetailsAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return Result.Failure<BasicOrganizationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        return Result.Success(response);
    }
}
