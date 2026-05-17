namespace Modules.WorkItems.Domain.Repositories;

public interface IWorkItemRepository
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
