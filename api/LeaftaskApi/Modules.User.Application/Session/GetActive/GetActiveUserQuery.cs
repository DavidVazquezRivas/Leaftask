using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Session.GetActive;

public sealed record GetActiveUserQuery : IQuery<Result<ActiveUserResponse>>;
