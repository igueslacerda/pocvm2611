using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using pocservicedn.Attributes;

namespace pocservicedn.Middleware;

public sealed class AuthenticatedUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requiresUserId = context.GetEndpoint()?.Metadata.GetMetadata<AuthenticatedUserAttribute>() is not null;
        if (!requiresUserId)
        {
            await next(context);
            return;
        }

        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (context.User.Identity?.IsAuthenticated != true ||
            !int.TryParse(userIdClaim, NumberStyles.None, CultureInfo.InvariantCulture, out var userId) ||
            userId <= 0)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Autenticação inválida."
            });
            return;
        }

        context.Items[AuthenticatedUserContextExtensions.UserIdItemKey] = userId;
        await next(context);
    }
}
