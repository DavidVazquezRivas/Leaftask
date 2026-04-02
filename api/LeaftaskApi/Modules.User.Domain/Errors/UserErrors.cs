using BuildingBlocks.Domain.Result;

namespace Modules.Users.Domain.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new("User.NotFound", "The specified user was not found.", 404);
}
