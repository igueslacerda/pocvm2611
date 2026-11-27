using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocservicedn.Entities;

[Table("ac_usuario", Schema = "public")]
public sealed class AcUsuarioEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column("login")]
    public required string Login { get; set; }

    [Required, MaxLength(255)]
    [Column("senha")]
    public required string Senha { get; set; }

    [Required]
    [Column("id_pessoa")]
    public int IdPessoa { get; set; }
}
