using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Chats.Application.Chats.List;

public sealed class ListChatsQueryHandler(
    IListChatsQueryService queryService,
    IUserContext userContext) : IQueryHandler<ListChatsQuery, Result<List<ChatDto>>>
{
    public async Task<Result<List<ChatDto>>> Handle(ListChatsQuery query, CancellationToken cancellationToken)
    {
        List<ChatDto> chats = await queryService.ListChatsAsync(userContext.UserId, cancellationToken);
        return Result.Success(chats);
    }
}
