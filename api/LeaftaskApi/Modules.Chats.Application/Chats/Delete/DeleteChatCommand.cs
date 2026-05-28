using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Chats.Application.Chats.Delete;

public sealed record DeleteChatCommand(Guid ChatId) : ICommand<Result>;
