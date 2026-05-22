using BuildingBlocks.Domain.Result;

namespace Modules.Users.Domain.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new("User.NotFound", "The specified user was not found.", 404);

    #region 401 Errors

    public static readonly Error InvalidGoogleToken =
        new("Session.OAuth.Google.InvalidToken", "The provided Google token is invalid.", 401);

    public static readonly Error InvalidRefreshToken =
        new("Session.Refresh.InvalidToken", "The provided refresh token is invalid", 401);

    public static readonly Error Unauthenticated =
        new("Session.Unauthenticated", "You don't have a valid session", 401);

    #endregion
}
