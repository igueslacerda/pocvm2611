using System.ComponentModel.DataAnnotations;

namespace pocservicedn.Contracts;

/// <summary>Dados necessários para alterar a própria senha.</summary>
public sealed record ChangePasswordRequest
{
    [Required, MaxLength(50)]
    public required string Login { get; init; }

    [Required, MaxLength(128)]
    public required string SenhaAtual { get; init; }

    [Required, MinLength(8), MaxLength(128)]
    public required string NovaSenha { get; init; }
}
