namespace Modules.Users.Application.Session.GetActive;

public sealed record ActiveUserResponse(Guid Id, string FirstName, string LastName, string Email);
