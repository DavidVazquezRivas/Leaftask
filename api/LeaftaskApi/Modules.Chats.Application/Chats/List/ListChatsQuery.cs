using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Chats.Application.Chats.List;

public sealed record ListChatsQuery : IQuery<Result<List<ChatDto>>>;
