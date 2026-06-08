using System.ComponentModel;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Create;
using Modules.Chats.Application.Chats.List;
using Modules.Chats.Application.Chats.Messages.List;
using Modules.Chats.Application.Chats.Messages.Send;

namespace Modules.Chats.DrivingInfrastructure.Tools;

public sealed class ChatMessagingAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("ListChats")]
    [Description(
        "Returns all chats the agent participates in. Use this to find the chatId needed before sending a message.")]
    public async Task<string> ListChatsAsync(CancellationToken cancellationToken = default)
    {
        Result<List<ChatDto>> result = await sender.Send(new ListChatsQuery(), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(ListChatsAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value.Select(c => new
        {
            c.Id,
            c.Name,
            c.Type,
            c.OtherParticipantId,
            LastMessage = c.LastMessage?.Content
        }));
    }

    [KernelFunction("GetChatMessages")]
    [Description("Retrieves recent messages from a specific chat. Use this to read what a user has replied.")]
    public async Task<string> GetChatMessagesAsync(
        [Description("The unique identifier (GUID) of the chat.")]
        Guid chatId,
        [Description("Maximum number of messages to retrieve. Default is 20.")]
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        Result<PaginatedResult<ChatMessageDto>> result = await sender.Send(
            new ListChatMessagesQuery(chatId, limit, null, []),
            cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(GetChatMessagesAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value.Items.Select(m => new
        {
            m.Id,
            m.Content,
            m.Timestamp,
            SenderName = m.Sender?.Name,
            SenderType = m.Sender?.Type
        }));
    }

    [KernelFunction("SendChatMessage")]
    [Description(
        "Sends a message to an existing chat. Use this to communicate directly with a user — for questions, " +
        "notifications, or follow-ups that require their reply. Prefer this over AddWorkItemComment when the " +
        "goal is to start or continue a conversation with a specific person.")]
    public async Task<string> SendMessageAsync(
        [Description("The unique identifier (GUID) of the chat to send the message to.")]
        Guid chatId,
        [Description("The message content to send.")]
        string content,
        CancellationToken cancellationToken = default)
    {
        Result<ChatMessageDto> result = await sender.Send(
            new SendMessageCommand(chatId, content),
            cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(SendMessageAsync), result.Error.Description);

        return formatter.FormatMessage($"[SUCCESS] Message sent. MessageId={result.Value.Id}");
    }

    [KernelFunction("FindOrCreateDirectChat")]
    [Description(
        "Finds an existing direct chat with a user or creates one if it doesn't exist. " +
        "Returns the chatId to use with SendChatMessage. " +
        "Pass the 'userId' field from GetProjectMembers or SearchUsers as the userId parameter.")]
    public async Task<string> FindOrCreateDirectChatAsync(
        [Description(
            "The global user ID to start a chat with. " +
            "Use the 'userId' field from GetProjectMembers, or the 'id' field from SearchUsers.")]
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // First check if a direct chat with this user already exists
        Result<List<ChatDto>> listResult = await sender.Send(new ListChatsQuery(), cancellationToken);
        if (listResult.IsFailure)
            return formatter.FormatFailure(nameof(FindOrCreateDirectChatAsync), listResult.Error.Description);

        ChatDto? existing = listResult.Value.FirstOrDefault(c =>
            c.Type == "Direct" && c.OtherParticipantId == userId);

        if (existing is not null)
            return formatter.FormatResponse(new { ChatId = existing.Id, existing.Name, Status = "existing" });

        // Create a new direct chat
        Result<ChatDto> createResult = await sender.Send(
            new CreateChatCommand(userId, "User"),
            cancellationToken);

        if (createResult.IsFailure)
            return formatter.FormatFailure(nameof(FindOrCreateDirectChatAsync), createResult.Error.Description);

        return formatter.FormatResponse(new
            { ChatId = createResult.Value.Id, createResult.Value.Name, Status = "created" });
    }
}
