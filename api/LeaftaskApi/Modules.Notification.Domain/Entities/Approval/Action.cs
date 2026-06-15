namespace Modules.Notification.Domain.Entities.Approval;

public sealed class Action
{
    private Action() { }

    public Action(Guid id, string code)
    {
        Id = id;
        Code = code;
    }

    public Guid Id { get; }
    public string Code { get; }
}
