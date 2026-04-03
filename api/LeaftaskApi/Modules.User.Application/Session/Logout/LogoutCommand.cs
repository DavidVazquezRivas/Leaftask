using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Session.Logout;

public record LogoutCommand : ICommand<Result>;
