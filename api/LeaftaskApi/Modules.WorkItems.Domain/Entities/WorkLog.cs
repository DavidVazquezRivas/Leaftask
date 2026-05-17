using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities;

public sealed class WorkLog : Entity
{
    private WorkLog() { }

    public WorkLog(Guid id, DateTime date, decimal hours, string comment, WorkItem workItem, UserReadModel user)
    {
        Id = id;
        Date = date;
        Hours = hours;
        Comment = comment;
        WorkItem = workItem;
        User = user;
    }

    public Guid Id { get; }
    public DateTime Date { get; }
    public decimal Hours { get; }
    public string Comment { get; }
    public WorkItem WorkItem { get; }
    public UserReadModel User { get; }
}
