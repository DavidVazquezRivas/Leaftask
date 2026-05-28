using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Chats.Application.Chats;

namespace Modules.Chats.Application.Chats.Poll;

public sealed class PollNewMessagesCommandHandler(
    IPollMessagesService pollMessagesService,
    IUserContext userContext) : ICommandHandler<PollNewMessagesCommand, Result<List<ChatMessageDto>>>
{
    public async Task<Result<List<ChatMessageDto>>> Handle(PollNewMessagesCommand command, CancellationToken cancellationToken)
    {
        List<ChatMessageDto> messages = await pollMessagesService.PollAsync(userContext.UserId, cancellationToken);
        return Result.Success(messages);
    }
}
