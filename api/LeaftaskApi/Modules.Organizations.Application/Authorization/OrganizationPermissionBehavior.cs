using System.Reflection;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using MediatR;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Authorization;

public sealed class OrganizationPermissionBehavior<TRequest, TResponse>(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IUserContext userContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IOrganizationPermissionRequest permissionRequest)
        {
            return await next(cancellationToken);
        }

        RequireOrganizationPermissionAttribute? permissionAttribute = request.GetType()
            .GetCustomAttribute<RequireOrganizationPermissionAttribute>();

        if (permissionAttribute is null)
        {
            return await next(cancellationToken);
        }

        Organization? organization = await organizationRepository.GetByIdAsync(permissionRequest.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return CreateFailureResponse(OrganizationErrors.OrganizationNotFound);
        }

        IReadOnlyCollection<OrganizationPermission> availablePermissions =
            await organizationPermissionRepository.GetAllAsync(cancellationToken);

        OrganizationPermission? requiredPermission = availablePermissions.FirstOrDefault(permission =>
            permission.Name.Equals(permissionAttribute.PermissionName, StringComparison.OrdinalIgnoreCase));

        if (requiredPermission is null)
        {
            return CreateFailureResponse(OrganizationErrors.OrganizationPermissionNotFound);
        }

        OrganizationInvitation? invitation = organization.Invitations.FirstOrDefault(inv =>
            inv.UserId == userContext.UserId && inv.Status == InvitationStatus.Accepted);

        if (invitation is null)
        {
            return CreateFailureResponse(OrganizationErrors.OrganizationMembershipRequired);
        }

        OrganizationRole? role = organization.Roles.FirstOrDefault(role => role.Id == invitation.OrganizationRoleId);
        OrganizationRolePermission? rolePermission = role?.Permissions.FirstOrDefault(permission =>
            permission.OrganizationPermissionId == requiredPermission.Id);

        if (rolePermission?.Level == PermissionLevel.Full)
        {
            return await next(cancellationToken);
        }

        if (rolePermission?.Level == PermissionLevel.Supervised)
        {
            organization.Raise(new OrganizationPermissionActionRequestedDomainEvent(
                organization.Id,
                userContext.UserId,
                requiredPermission.Name,
                request.GetType().Name));

            await organizationRepository.SaveChangesAsync(cancellationToken);

            return CreateFailureResponse(OrganizationErrors.OrganizationPermissionApprovalRequired);
        }

        return CreateFailureResponse(OrganizationErrors.OrganizationPermissionDenied);
    }

    private static TResponse CreateFailureResponse(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];
            MethodInfo failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method => method.Name == nameof(Result.Failure)
                                  && method.IsGenericMethodDefinition
                                  && method.GetParameters().Length == 1)
                .MakeGenericMethod(valueType);

            object response = failureMethod.Invoke(null, [error])!;
            return (TResponse)response;
        }

        throw new InvalidOperationException(
            $"{nameof(OrganizationPermissionBehavior<TRequest, TResponse>)} supports only Result and Result<T> responses.");
    }
}
