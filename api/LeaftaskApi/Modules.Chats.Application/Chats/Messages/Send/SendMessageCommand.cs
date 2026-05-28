using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;

namespace Modules.Chats.Application.Chats.Messages.Send;

public sealed record SendMessageCommand(Guid ChatId, string Content) : ICommand<Result<ChatMessageDto>>;
