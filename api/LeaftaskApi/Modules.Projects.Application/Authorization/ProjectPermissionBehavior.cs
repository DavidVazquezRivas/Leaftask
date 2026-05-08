using System.Reflection;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using MediatR;
using Modules.Projects.Domain.Errors;

namespace Modules.Projects.Application.Authorization;

public sealed class ProjectPermissionBehavior<TRequest, TResponse>(
    IProjectPermissionChecker projectPermissionChecker,
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
        {
            return await next(cancellationToken);
        }

        RequireProjectPermissionAttribute? permissionAttribute = request.GetType()
            .GetCustomAttribute<RequireProjectPermissionAttribute>();

        if (permissionAttribute is null)
        {
            return await next(cancellationToken);
        }

        ProjectPermissionCheckStatus status = await projectPermissionChecker.CheckAsync(
            permissionRequest.ProjectId,
            userContext.UserId,
            permissionAttribute.PermissionName,
            cancellationToken);

        if (status == ProjectPermissionCheckStatus.Full)
        {
            return await next(cancellationToken);
        }

        return CreateFailureResponse(status switch
        {
            ProjectPermissionCheckStatus.ProjectNotFound => ProjectErrors.ProjectNotFound,
            _ => ProjectErrors.AccessDenied
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

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException(
            $"{nameof(ProjectPermissionBehavior<TRequest, TResponse>)} supports only Result and Result<T> responses.");
    }
}
