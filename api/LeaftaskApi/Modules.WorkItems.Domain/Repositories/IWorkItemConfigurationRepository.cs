using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.Domain.Repositories;

public interface IWorkItemConfigurationRepository
{
    Task<WorkItemStatus?> GetStatusByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkItemType?> GetTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
