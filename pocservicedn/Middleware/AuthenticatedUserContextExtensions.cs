namespace pocservicedn.Middleware;

internal static class AuthenticatedUserContextExtensions
{
    internal const string UserIdItemKey = "AuthenticatedUserId";

    public static int GetAuthenticatedUserId(this HttpContext context) =>
        context.Items.TryGetValue(UserIdItemKey, out var value) && value is int userId
            ? userId
            : throw new InvalidOperationException("Authenticated user ID is not available in the request context.");
}
