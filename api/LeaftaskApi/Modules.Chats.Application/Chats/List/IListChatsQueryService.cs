namespace Modules.Chats.Application.Chats.List;

public interface IListChatsQueryService
{
    Task<List<ChatDto>> ListChatsAsync(Guid userId, CancellationToken cancellationToken = default);
}
