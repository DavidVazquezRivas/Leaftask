using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;

namespace Modules.Chats.Application.Chats.Messages.List;

public sealed record ListChatMessagesQuery(
    Guid ChatId,
    int Limit,
    string? Cursor,
    IReadOnlyList<string> Sort) : IQuery<Result<PaginatedResult<ChatMessageDto>>>;
