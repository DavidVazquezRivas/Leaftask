using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Members.GetMembers;

public sealed class GetOrganizationMembersQueryHandler(
    IOrganizationRepository organizationRepository,
    IGetOrganizationMembersQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetOrganizationMembersQuery, Result<PaginatedResult<OrganizationMemberDto>>>
{
    public async Task<Result<PaginatedResult<OrganizationMemberDto>>> Handle(
        GetOrganizationMembersQuery request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<PaginatedResult<OrganizationMemberDto>>(OrganizationErrors.OrganizationNotFound);
        }

        bool isMember = organization.Invitations.Any(invitation =>
            invitation.UserId == userContext.UserId && invitation.Status == InvitationStatus.Accepted);

        if (!isMember)
        {
            return Result.Failure<PaginatedResult<OrganizationMemberDto>>(OrganizationErrors.OrganizationMembershipRequired);
        }

        PaginatedResult<OrganizationMemberDto> members = await service.GetOrganizationMembersAsync(
            request.OrganizationId,
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);

        return Result.Success(members);
    }
}
