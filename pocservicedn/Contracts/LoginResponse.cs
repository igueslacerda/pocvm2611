namespace pocservicedn.Contracts;

/// <summary>Token JWT emitido após autenticação bem-sucedida.</summary>
public sealed record LoginResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
