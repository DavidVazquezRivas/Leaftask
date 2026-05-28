using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;

namespace Modules.Chats.Application.Chats.Poll;

public sealed record PollNewMessagesCommand : ICommand<Result<List<ChatMessageDto>>>;
