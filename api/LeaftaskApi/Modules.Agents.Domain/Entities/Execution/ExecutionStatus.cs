namespace Modules.Agents.Domain.Entities.Execution;

public enum ExecutionStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Suspended = 4
}
