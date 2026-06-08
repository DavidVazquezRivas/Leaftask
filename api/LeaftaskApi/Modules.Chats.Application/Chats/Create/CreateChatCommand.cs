using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;

namespace Modules.Chats.Application.Chats.Create;

public sealed record CreateChatCommand(
    Guid OtherParticipantId,
    string OtherParticipantType,
    string SelfParticipantType = "person") : ICommand<Result<ChatDto>>;
