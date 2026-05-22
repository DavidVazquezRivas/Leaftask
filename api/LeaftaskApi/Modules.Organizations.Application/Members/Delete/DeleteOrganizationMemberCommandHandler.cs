using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Members.Delete;

public sealed class DeleteOrganizationMemberCommandHandler(
    IOrganizationRepository organizationRepository)
    : ICommandHandler<DeleteOrganizationMemberCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationMemberCommand request, CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationNotFound);
        }

        Result removeResult = organization.RemoveMember(request.MemberId);
        if (removeResult.IsFailure)
        {
            return Result.Failure(removeResult.Error);
        }

        await organizationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
