using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Session.Refresh;

public record RefreshSessionCommand(string RefreshToken) : ICommand<Result<SessionResponse>>;
