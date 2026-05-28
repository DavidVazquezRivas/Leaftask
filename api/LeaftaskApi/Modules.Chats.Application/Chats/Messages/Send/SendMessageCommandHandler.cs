using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Chats.Messages.Send;

public sealed class SendMessageCommandHandler(
    IChatRepository chatRepository,
    IChatMessageRepository messageRepository,
    IUserReadModelRepository userReadModelRepository,
    IUserContext userContext) : ICommandHandler<SendMessageCommand, Result<ChatMessageDto>>
{
    public async Task<Result<ChatMessageDto>> Handle(SendMessageCommand command, CancellationToken cancellationToken)
    {
        Chat? chat = await chatRepository.GetByIdTrackedAsync(command.ChatId, cancellationToken);
        if (chat is null)
            return Result.Failure<ChatMessageDto>(ChatErrors.ChatNotFound);

        ChatParticipant? senderParticipant = await chatRepository.GetParticipantTrackedAsync(
            command.ChatId, userContext.UserId, cancellationToken);
        if (senderParticipant is null)
            return Result.Failure<ChatMessageDto>(ChatErrors.NotParticipant);

        ChatMessage message = ChatMessage.Create(
            Guid.NewGuid(), command.Content, DateTime.UtcNow,
            MessageStatus.Pending, chat, senderParticipant);

        await messageRepository.AddAsync(message, cancellationToken);
        await messageRepository.SaveChangesAsync(cancellationToken);

        string senderName;
        string senderType;

        if (senderParticipant.ParticipantType == ParticipantType.Agent)
        {
            senderName = "AI Assistant";
            senderType = "agent";
        }
        else
        {
            UserReadModel? user = await userReadModelRepository.GetByIdAsync(userContext.UserId, cancellationToken);
            senderName = user is null ? "" : $"{user.FirstName} {user.LastName}".Trim();
            senderType = "person";
        }

        ChatSenderDto sender = new(senderParticipant.ParticipantId, senderName, senderType);
        ChatMessageDto dto = new(
            message.Id, command.ChatId, message.Content, message.CreatedAt,
            MapStatus(message.Status), sender);

        return Result.Success(dto);
    }

    private static string MapStatus(MessageStatus status) => status switch
    {
        MessageStatus.Pending => "pending",
        MessageStatus.Delivered => "delivered",
        MessageStatus.Read => "read",
        _ => status.ToString()
    };
}
