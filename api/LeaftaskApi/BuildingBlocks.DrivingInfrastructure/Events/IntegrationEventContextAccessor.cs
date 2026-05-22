namespace BuildingBlocks.DrivingInfrastructure.Events;

internal sealed class IntegrationEventContextAccessor : IIntegrationEventContextAccessor
{
    public Guid? CurrentMessageId { get; set; }
}
