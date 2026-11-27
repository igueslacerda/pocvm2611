using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using pocservicedn.Attributes;
using pocservicedn.Contracts;
using pocservicedn.Middleware;
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

    [AuthenticatedUser]
    [HttpPatch("senha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authenticationService.ChangePasswordAsync(
            HttpContext.GetAuthenticatedUserId(),
            request,
            cancellationToken);
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

    [AuthenticatedUser]
    [HttpGet("perfil")]
    [ProducesResponseType<UserProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile(CancellationToken cancellationToken)
    {
        var profile = await authenticationService.ObterPerfilAsync(
            HttpContext.GetAuthenticatedUserId(),
            cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }
}
