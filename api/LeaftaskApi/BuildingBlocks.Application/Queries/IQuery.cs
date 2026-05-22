using MediatR;

namespace BuildingBlocks.Application.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse>;
