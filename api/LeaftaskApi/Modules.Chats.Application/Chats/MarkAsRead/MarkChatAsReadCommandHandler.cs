using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Chats.MarkAsRead;

public sealed class MarkChatAsReadCommandHandler(
    IChatRepository chatRepository,
    IChatMessageRepository chatMessageRepository,
    IUserContext userContext) : ICommandHandler<MarkChatAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkChatAsReadCommand command, CancellationToken cancellationToken)
    {
        ChatParticipant? participant = await chatRepository.GetParticipantAsync(
            command.ChatId, userContext.UserId, cancellationToken);

        if (participant is null)
            return Result.Failure(ChatErrors.NotParticipant);

        await chatMessageRepository.MarkChatMessagesAsReadAsync(
            command.ChatId, userContext.UserId, cancellationToken);

        return Result.Success();
    }
}
