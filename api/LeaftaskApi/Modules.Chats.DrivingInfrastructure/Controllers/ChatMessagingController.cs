using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Messages.List;
using Modules.Chats.Application.Chats.Messages.Send;
using Modules.Chats.DrivingInfrastructure.Models.Requests;

namespace Modules.Chats.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/chats/{chatId:guid}/messages")]
public sealed class ChatMessagingController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> ListMessages(
        Guid chatId,
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] string[]? sort = null,
        CancellationToken cancellationToken = default)
    {
        Result<PaginatedResult<ChatMessageDto>> result = await Sender.Send(
            new ListChatMessagesQuery(chatId, limit, cursor, sort ?? []),
            cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result.Error);

        PaginationMeta pagination = new()
        {
            Limit = limit,
            NextCursor = result.Value.NextCursor,
            HasMore = result.Value.HasMore
        };

        return HandleResult(result, pagination: pagination);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(
        Guid chatId,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<ChatMessageDto> result = await Sender.Send(
            new SendMessageCommand(chatId, request.Content),
            cancellationToken);

        return HandleResult(result, StatusCodes.Status201Created);
    }
}
