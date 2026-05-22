namespace BuildingBlocks.DrivingInfrastructure.Events;

public interface IIntegrationEventContextAccessor
{
    Guid? CurrentMessageId { get; set; }
}
