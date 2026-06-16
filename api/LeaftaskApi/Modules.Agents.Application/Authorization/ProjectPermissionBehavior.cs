using System.Reflection;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using MediatR;
using Modules.Agents.Domain.Errors;
using Modules.Projects.Integration;

namespace Modules.Agents.Application.Authorization;

public sealed class ProjectPermissionBehavior<TRequest, TResponse>(
    IProjectPermissionService projectPermissionService,
    IUserContext userContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IProjectPermissionRequest permissionRequest)
            return await next(cancellationToken);

        RequireProjectPermissionAttribute? attribute = request.GetType()
            .GetCustomAttribute<RequireProjectPermissionAttribute>();

        if (attribute is null)
            return await next(cancellationToken);

        ProjectPermissionCheckStatus status = await projectPermissionService.CheckPermissionAsync(
            permissionRequest.ProjectId,
            userContext.UserId,
            attribute.PermissionName,
            cancellationToken);

        if (status == ProjectPermissionCheckStatus.Full)
            return await next(cancellationToken);

        Error error = status == ProjectPermissionCheckStatus.ProjectNotFound
            ? AgentErrors.ProjectNotFound
            : AgentErrors.AccessDenied;

        return CreateFailureResponse(error);
    }

    private static TResponse CreateFailureResponse(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];
            MethodInfo failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m => m.Name == nameof(Result.Failure)
                             && m.IsGenericMethodDefinition
                             && m.GetParameters().Length == 1)
                .MakeGenericMethod(valueType);

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException(
            $"{nameof(ProjectPermissionBehavior<TRequest, TResponse>)} supports only Result and Result<T> responses.");
    }
}
