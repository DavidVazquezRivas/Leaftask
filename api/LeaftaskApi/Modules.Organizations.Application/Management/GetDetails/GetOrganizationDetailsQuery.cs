using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Management.GetDetails;

public sealed record GetOrganizationDetailsQuery(Guid Id)
    : IQuery<Result<BasicOrganizationResponse>>;
