using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pocservicedn.Contracts;
using pocservicedn.Services;

namespace pocservicedn.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authenticationService.LoginAsync(request, cancellationToken);
        return response is null
            ? Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Credenciais inválidas."
            })
            : Ok(response);
    }

    [Authorize]
    [HttpPatch("senha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var result = await authenticationService.ChangePasswordAsync(userId, request, cancellationToken);
        return result switch
        {
            ChangePasswordResult.Success => NoContent(),
            ChangePasswordResult.UserNotFound => NotFound(),
            ChangePasswordResult.LoginMismatch => Forbid(),
            ChangePasswordResult.InvalidCurrentPassword => BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "A senha atual está incorreta."
            }),
            _ => throw new InvalidOperationException("Resultado de alteração de senha não reconhecido.")
        };
    }

    [Authorize]
    [HttpGet("perfil")]
    [ProducesResponseType<UserProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile(CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var profile = await authenticationService.ObterPerfilAsync(userId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }
}
