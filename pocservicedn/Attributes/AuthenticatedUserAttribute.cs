using Microsoft.AspNetCore.Authorization;

namespace pocservicedn.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AuthenticatedUserAttribute : AuthorizeAttribute;
