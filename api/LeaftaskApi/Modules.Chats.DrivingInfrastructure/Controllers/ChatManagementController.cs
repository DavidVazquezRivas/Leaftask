using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Create;
using Modules.Chats.Application.Chats.Delete;
using Modules.Chats.Application.Chats.List;
using Modules.Chats.Application.Chats.MarkAsRead;
using Modules.Chats.Application.Chats.Poll;
using Modules.Chats.DrivingInfrastructure.Models.Requests;

namespace Modules.Chats.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/chats")]
public sealed class ChatManagementController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> ListChats(CancellationToken cancellationToken = default)
    {
        Result<List<ChatDto>> result = await Sender.Send(new ListChatsQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateChat(
        [FromBody] CreateChatRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<ChatDto> result = await Sender.Send(
            new CreateChatCommand(request.OtherParticipantId ?? Guid.Empty, request.OtherParticipantType),
            cancellationToken);

        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpDelete("{chatId:guid}")]
    public async Task<IActionResult> DeleteChat(Guid chatId, CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(new DeleteChatCommand(chatId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{chatId:guid}/read")]
    public async Task<IActionResult> MarkChatAsRead(Guid chatId, CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(new MarkChatAsReadCommand(chatId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("polling")]
    public async Task<IActionResult> PollNewMessages(CancellationToken cancellationToken = default)
    {
        Result<List<ChatMessageDto>> result = await Sender.Send(new PollNewMessagesCommand(), cancellationToken);
        return HandleResult(result);
    }
}
