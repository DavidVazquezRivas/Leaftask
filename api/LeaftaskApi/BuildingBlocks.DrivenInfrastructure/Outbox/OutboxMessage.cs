namespace BuildingBlocks.DrivenInfrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();

    // Will contain AssemblyQualifiedName of the event type
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }

    public static OutboxMessage Create(string type, string content) =>
        new()
        {
            Type = type,
            Content = content
        };

    public void Process() => ProcessedAt = DateTime.UtcNow;
}
