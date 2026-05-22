namespace BuildingBlocks.DrivenInfrastructure.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;

    public static InboxMessage Create(Guid id, string type, string content) =>
        new()
        {
            Id = id,
            Type = type,
            Content = content
        };
}
