using System.ComponentModel.DataAnnotations;
using pocservicedn.Enums;

namespace pocservicedn.Contracts;

/// <summary>Credenciais usadas para iniciar uma sessão.</summary>
public sealed record LoginRequest
{
    [Required, MaxLength(50)]
    public required string Login { get; init; }

    [Required, MaxLength(128)]
    public required string Senha { get; init; }

    [EnumDataType(typeof(AcLogOrigemEnum))]
    public required AcLogOrigemEnum Origem { get; init; }
}
