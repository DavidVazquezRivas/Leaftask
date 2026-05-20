namespace Modules.WorkItems.Application.WorkLogs;

public sealed record WorkLogDto(
    Guid Id,
    float Dedication,
    DateOnly Date,
    WorkLogUserDto User,
    string Description);

public sealed record WorkLogUserDto(Guid Id, string FirstName, string LastName);
