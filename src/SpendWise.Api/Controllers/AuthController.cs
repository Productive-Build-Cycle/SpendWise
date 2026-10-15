using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Api.Contracts.Authentication;
using SpendWise.Application.Features.Commands.Authentication.Login;
using SpendWise.Application.Features.Commands.Authentication.Refresh;
using SpendWise.Application.Features.Commands.Authentication.Register;

namespace SpendWise.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "__Secure-RefreshToken";

    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";

        Response.Cookies.Append(RefreshTokenCookieName, result.RawRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth",
                Expires = new DateTimeOffset(result.RefreshTokenExpiresAtUtc)
            });

        var response = new LoginResponse(
            AccessToken: result.AccessToken,
            TokenType: "Bearer",
            ExpiresIn: result.ExpiresInSeconds);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var rawRefreshToken) ||
            string.IsNullOrWhiteSpace(rawRefreshToken))
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new RefreshTokenCommand(rawRefreshToken), cancellationToken);

        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.AccessToken) ||
            string.IsNullOrWhiteSpace(result.RawRefreshToken) || result.RefreshTokenExpiresAtUtc is null)
        {
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth"
            });

            return Unauthorized();
        }

        Response.Cookies.Append(RefreshTokenCookieName, result.RawRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth",
                Expires = result.RefreshTokenExpiresAtUtc.Value
            });

        return Ok(new
        {
            AccessToken = result.AccessToken,
            TokenType = "Bearer",
            ExpiresIn = result.ExpiresInSeconds
        });
    }
}