using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Messages.List;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivenInfrastructure.Queries;

public sealed class ListChatMessagesQueryService(ChatsDbContext dbContext) : IListChatMessagesQueryService
{
    private const string IdSort = "id:asc";

    private static readonly CursorSortFieldDefinition<ChatMessageRow> TimestampField = new(
        "timestamp", r => r.Timestamp,
        v => DateTime.Parse(v, CultureInfo.InvariantCulture),
        v => ((DateTime)v).ToString("O", CultureInfo.InvariantCulture));

    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<ChatMessageRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<ChatMessageRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", r => r.Id, v => Guid.Parse(v), v => ((Guid)v).ToString("D")),
            ["timestamp"] = TimestampField,
            ["createdAt"] = TimestampField
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["timestamp:asc", "id:asc"];

    public async Task<PaginatedResult<ChatMessageDto>> GetMessagesAsync(
        Guid chatId,
        Guid userId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<ChatMessage> messages = await dbContext.ChatMessages
            .Include(m => m.Sender)
            .Where(m => EF.Property<Guid>(m, "chat_id") == chatId)
            .ToListAsync(cancellationToken);

        foreach (ChatMessage message in messages.Where(m => m.Sender.ParticipantId != userId))
        {
            if (message.Status == MessageStatus.Pending)
                message.MarkAsDelivered();
            else if (message.Status == MessageStatus.Delivered)
                message.MarkAsRead();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        List<Guid> personSenderIds = messages
            .Where(m => m.Sender.ParticipantType == ParticipantType.User)
            .Select(m => m.Sender.ParticipantId)
            .Distinct()
            .ToList();

        Dictionary<Guid, (string FirstName, string LastName)> userNames = await dbContext.UserReadModels
            .AsNoTracking()
            .Where(u => personSenderIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => (u.FirstName, u.LastName), cancellationToken);

        List<ChatMessageRow> rows = messages
            .Select(m => new ChatMessageRow(
                m.Id, chatId, m.Content, m.CreatedAt, m.Status,
                m.Sender.ParticipantId, m.Sender.ParticipantType))
            .ToList();

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            rows,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            row => new ChatMessageDto(
                row.Id,
                row.ChatId,
                row.Content,
                row.Timestamp,
                MapStatus(row.Status),
                BuildSender(row, userNames)));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort)
    {
        IReadOnlyCollection<string> baseSort = sort.Count == 0 ? DefaultSort : sort;
        return baseSort.Any(s => s.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? baseSort
            : [.. baseSort, IdSort];
    }

    private static string MapStatus(MessageStatus status) => status switch
    {
        MessageStatus.Pending => "pending",
        MessageStatus.Delivered => "delivered",
        MessageStatus.Read => "read",
        _ => status.ToString()
    };

    private static ChatSenderDto BuildSender(
        ChatMessageRow row,
        Dictionary<Guid, (string FirstName, string LastName)> userNames)
    {
        if (row.SenderType == ParticipantType.Agent)
            return new ChatSenderDto(row.SenderParticipantId, "AI Assistant", "agent");

        userNames.TryGetValue(row.SenderParticipantId, out (string FirstName, string LastName) info);
        string name = $"{info.FirstName} {info.LastName}".Trim();
        return new ChatSenderDto(row.SenderParticipantId, name, "person");
    }

    private sealed record ChatMessageRow(
        Guid Id,
        Guid ChatId,
        string Content,
        DateTime Timestamp,
        MessageStatus Status,
        Guid SenderParticipantId,
        ParticipantType SenderType);
}
