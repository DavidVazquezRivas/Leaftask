using System.Reflection;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using MediatR;
using Modules.Projects.Domain.Errors;

namespace Modules.Projects.Application.Authorization;

public sealed class OrganizationPermissionBehavior<TRequest, TResponse>(
    IOrganizationPermissionChecker organizationPermissionChecker,
    IUserContext userContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IOrganizationPermissionRequest permissionRequest ||
            permissionRequest.OrganizationId is null)
        {
            return await next(cancellationToken);
        }

        RequireOrganizationPermissionAttribute? permissionAttribute = request.GetType()
            .GetCustomAttribute<RequireOrganizationPermissionAttribute>();

        if (permissionAttribute is null)
        {
            return await next(cancellationToken);
        }

        OrganizationPermissionCheckStatus checkResult = await organizationPermissionChecker.CheckAsync(
            permissionRequest.OrganizationId.Value,
            userContext.UserId,
            permissionAttribute.PermissionName,
            cancellationToken);

        if (checkResult == OrganizationPermissionCheckStatus.Full)
        {
            return await next(cancellationToken);
        }

        return CreateFailureResponse(checkResult switch
        {
            OrganizationPermissionCheckStatus.Supervised => ProjectErrors.OrganizationPermissionApprovalRequired,
            OrganizationPermissionCheckStatus.Denied => ProjectErrors.OrganizationPermissionDenied,
            OrganizationPermissionCheckStatus.MembershipRequired => ProjectErrors.OrganizationMembershipRequired,
            OrganizationPermissionCheckStatus.PermissionNotFound => ProjectErrors.OrganizationPermissionNotFound,
            OrganizationPermissionCheckStatus.OrganizationNotFound => ProjectErrors.OrganizationNotFound,
            _ => ProjectErrors.OrganizationPermissionDenied
        });
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
