using System.Reflection;
using System.Text.Json;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using MediatR;

namespace BuildingBlocks.Application.Authorization;

public sealed class ProjectPermissionBehavior<TRequest, TResponse>(
    IProjectPermissionService projectPermissionService,
    IUserContext userContext,
    IProjectPermissionReplayContext replayContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly Error AccessDenied =
        new("Project.Permission.Denied", "You do not have permission to perform this operation.", 403);

    private static readonly Error ApprovalRequired =
        new("Project.Permission.ApprovalRequired", "The action requires approval before it can be executed.", 403);

    private static readonly Error ProjectNotFound =
        new("Project.NotFound", "The specified project was not found.", 404);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (replayContext.IsReplaying)
            return await next(cancellationToken);

        if (request is not IProjectPermissionRequest permissionRequest)
            return await next(cancellationToken);

        RequireProjectPermissionAttribute? permissionAttribute = request.GetType()
            .GetCustomAttribute<RequireProjectPermissionAttribute>();

        if (permissionAttribute is null)
            return await next(cancellationToken);

        ProjectPermissionCheckStatus status = await projectPermissionService.CheckPermissionAsync(
            permissionRequest.ProjectId,
            userContext.UserId,
            permissionAttribute.PermissionName,
            cancellationToken);

        if (status == ProjectPermissionCheckStatus.Full)
            return await next(cancellationToken);

        if (status == ProjectPermissionCheckStatus.Supervised)
        {
            await projectPermissionService.RequestApprovalAsync(
                permissionRequest.ProjectId,
                userContext.UserId,
                permissionAttribute.PermissionName,
                request.GetType().AssemblyQualifiedName!,
                JsonSerializer.Serialize(request, request.GetType()),
                cancellationToken);

            return CreateFailureResponse(ApprovalRequired);
        }

        return CreateFailureResponse(status == ProjectPermissionCheckStatus.ProjectNotFound
            ? ProjectNotFound
            : AccessDenied);
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
