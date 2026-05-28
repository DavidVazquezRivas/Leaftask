using BuildingBlocks.Application.Queries;
using Modules.Chats.Application.Chats;

namespace Modules.Chats.Application.Chats.Messages.List;

public interface IListChatMessagesQueryService
{
    Task<PaginatedResult<ChatMessageDto>> GetMessagesAsync(
        Guid chatId,
        Guid userId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
