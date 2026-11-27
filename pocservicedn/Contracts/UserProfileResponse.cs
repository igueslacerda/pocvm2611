namespace pocservicedn.Contracts;

/// <summary>Dados do usuário autenticado e da pessoa relacionada.</summary>
public sealed record UserProfileResponse(int Id, string Login, PessoaResponse Pessoa);
