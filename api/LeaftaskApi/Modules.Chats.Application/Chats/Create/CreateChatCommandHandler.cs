using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Chats.Create;

public sealed class CreateChatCommandHandler(
    IChatRepository chatRepository,
    IUserReadModelRepository userReadModelRepository,
    IAgentReadModelRepository agentReadModelRepository,
    IUserContext userContext) : ICommandHandler<CreateChatCommand, Result<ChatDto>>
{
    public async Task<Result<ChatDto>> Handle(CreateChatCommand command, CancellationToken cancellationToken)
    {
        ParticipantType selfType = command.SelfParticipantType == "agent"
            ? ParticipantType.Agent
            : ParticipantType.User;

        ParticipantType otherType = command.OtherParticipantType == "agent"
            ? ParticipantType.Agent
            : ParticipantType.User;

        if (otherType == ParticipantType.User)
        {
            bool otherExists = await userReadModelRepository.ExistsByIdAsync(
                command.OtherParticipantId, cancellationToken);
            if (!otherExists)
                return Result.Failure<ChatDto>(ChatErrors.ParticipantNotFound);
        }

        bool alreadyExists = await chatRepository.ExistsBetweenParticipantsAsync(
            userContext.UserId, command.OtherParticipantId, otherType, cancellationToken);
        if (alreadyExists)
            return Result.Failure<ChatDto>(ChatErrors.AlreadyExists);

        DateTime now = DateTime.UtcNow;
        Chat chat = Chat.Create(Guid.NewGuid(), now);

        ChatParticipant selfParticipant = new(
            Guid.NewGuid(), userContext.UserId, selfType, now, chat);

        ChatParticipant otherParticipant = new(
            Guid.NewGuid(), command.OtherParticipantId, otherType, now, chat);

        await chatRepository.AddAsync(chat, cancellationToken);
        await chatRepository.AddParticipantAsync(selfParticipant, cancellationToken);
        await chatRepository.AddParticipantAsync(otherParticipant, cancellationToken);
        await chatRepository.SaveChangesAsync(cancellationToken);

        string name;
        string type;

        if (otherType == ParticipantType.Agent)
        {
            AgentReadModel? agent = await agentReadModelRepository.GetByIdAsync(
                command.OtherParticipantId, cancellationToken);
            name = agent?.Name ?? "AI Assistant";
            type = "agent";
        }
        else
        {
            UserReadModel? other = await userReadModelRepository.GetByIdAsync(
                command.OtherParticipantId, cancellationToken);
            name = other is null ? "" : $"{other.FirstName} {other.LastName}".Trim();
            type = "person";
        }

        Guid? otherParticipantId = command.OtherParticipantId;

        return Result.Success(new ChatDto(chat.Id, name, null, type, null, otherParticipantId, 0));
    }
}
