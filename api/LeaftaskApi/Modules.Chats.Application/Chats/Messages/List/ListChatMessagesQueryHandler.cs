using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Chats.Messages.List;

public sealed class ListChatMessagesQueryHandler(
    IListChatMessagesQueryService queryService,
    IChatRepository chatRepository,
    IUserContext userContext) : IQueryHandler<ListChatMessagesQuery, Result<PaginatedResult<ChatMessageDto>>>
{
    public async Task<Result<PaginatedResult<ChatMessageDto>>> Handle(
        ListChatMessagesQuery query, CancellationToken cancellationToken)
    {
        Chat? chat = await chatRepository.GetByIdAsync(query.ChatId, cancellationToken);
        if (chat is null)
            return Result.Failure<PaginatedResult<ChatMessageDto>>(ChatErrors.ChatNotFound);

        ChatParticipant? participant = await chatRepository.GetParticipantAsync(
            query.ChatId, userContext.UserId, cancellationToken);
        if (participant is null)
            return Result.Failure<PaginatedResult<ChatMessageDto>>(ChatErrors.NotParticipant);

        PaginatedResult<ChatMessageDto> result = await queryService.GetMessagesAsync(
            query.ChatId, userContext.UserId, query.Limit, query.Cursor, query.Sort, cancellationToken);

        return Result.Success(result);
    }
}
