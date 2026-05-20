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
    public DateTime Date { get; private set; }
    public decimal Hours { get; private set; }
    public string Comment { get; private set; }
    public WorkItem WorkItem { get; }
    public UserReadModel User { get; }

    public void Update(DateTime? date, decimal? hours, string? comment)
    {
        if (date.HasValue) Date = date.Value;
        if (hours.HasValue) Hours = hours.Value;
        if (comment is not null) Comment = comment;
    }
}
