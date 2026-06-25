using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Users.Application.Session;

namespace Modules.Users.Application.Session.DevLogin;

public sealed record DevLoginCommand(string Email) : ICommand<Result<SessionResponse>>;
