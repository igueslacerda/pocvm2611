namespace pocservicedn.Contracts;

/// <summary>Dados pessoais associados ao usuário autenticado.</summary>
public sealed record PessoaResponse(
    int Id,
    string Nome,
    string Sobrenome,
    DateOnly? DataNascimento,
    string? Email,
    string? Telefone,
    string? Endereco);
