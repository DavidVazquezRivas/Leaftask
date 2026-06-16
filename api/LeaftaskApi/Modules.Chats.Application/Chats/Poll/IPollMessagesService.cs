using Modules.Chats.Application.Chats;

namespace Modules.Chats.Application.Chats.Poll;

public interface IPollMessagesService
{
    Task<List<ChatMessageDto>> PollAsync(Guid userId, CancellationToken cancellationToken = default);
}
