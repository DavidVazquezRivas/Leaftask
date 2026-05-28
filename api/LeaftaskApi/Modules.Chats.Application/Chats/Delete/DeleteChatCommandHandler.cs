using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Chats.Delete;

public sealed class DeleteChatCommandHandler(
    IChatRepository chatRepository,
    IUserContext userContext) : ICommandHandler<DeleteChatCommand, Result>
{
    public async Task<Result> Handle(DeleteChatCommand command, CancellationToken cancellationToken)
    {
        Chat? chat = await chatRepository.GetByIdTrackedAsync(command.ChatId, cancellationToken);
        if (chat is null)
            return Result.Failure(ChatErrors.ChatNotFound);

        ChatParticipant? participant = await chatRepository.GetParticipantAsync(
            command.ChatId, userContext.UserId, cancellationToken);
        if (participant is null)
            return Result.Failure(ChatErrors.NotParticipant);

        chatRepository.Remove(chat);
        await chatRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
