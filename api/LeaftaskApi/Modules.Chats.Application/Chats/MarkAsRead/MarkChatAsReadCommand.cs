using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Chats.Application.Chats.MarkAsRead;

public sealed record MarkChatAsReadCommand(Guid ChatId) : ICommand<Result>;
