using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;

namespace BuildingBlocks.Application.Events;

public interface IIntegrationEventMapper
{
    IIntegrationEvent? Map(IDomainEvent domainEvent);
}
