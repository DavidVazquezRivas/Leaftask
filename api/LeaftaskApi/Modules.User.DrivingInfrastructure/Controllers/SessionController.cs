using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Session;
using Modules.Users.Application.Session.Logout;
using Modules.Users.Application.Session.OAuth.Google;
using Modules.Users.Application.Session.Refresh;
using Modules.Users.Domain.Errors;
using Modules.Users.DrivingInfrastructure.Models.Requests;
using Modules.Users.DrivingInfrastructure.Models.Responses;

namespace Modules.Users.DrivingInfrastructure.Controllers;

[Route("api/v1/session")]
public sealed class SessionController : ApiBaseController
{
    [AllowAnonymous]
    [HttpPost("oauth/google")]
    public async Task<IActionResult> LoginGoogle(
        [FromBody] LoginGoogleRequest request,
        CancellationToken cancellationToken)
    {
        LoginGoogleCommand command = new(request.Token);
        Result<SessionResponse> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        SessionResponse sessionResponse = result.Value;

        SetRefreshTokenCookie(sessionResponse.RefreshToken, sessionResponse.RefreshTokenExpiresAt);

        ApiResponse<AccessTokenResponse> response =
            BuildSuccessResponse(new AccessTokenResponse(sessionResponse.AccessToken));
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshSession(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out string currentRefreshToken))
        {
            return HandleFailure(UserErrors.InvalidRefreshToken);
        }

        RefreshSessionCommand command = new(currentRefreshToken);
        Result<SessionResponse> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            Response.Cookies.Delete("refreshToken");
            return HandleFailure(result.Error);
        }

        SessionResponse sessionResponse = result.Value;

        SetRefreshTokenCookie(sessionResponse.RefreshToken, sessionResponse.RefreshTokenExpiresAt);

        ApiResponse<AccessTokenResponse> response =
            BuildSuccessResponse(new AccessTokenResponse(sessionResponse.AccessToken));
        return Ok(response);
    }

    [HttpDelete("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        Result result = await Sender.Send(new LogoutCommand(), cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        Response.Cookies.Delete("refreshToken");

        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
