using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pocservicedn.Enums;

namespace pocservicedn.Entities;

[Table("ac_log", Schema = "public")]
public sealed class AcLogEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Required]
    [Column("acao")]
    public AcLogAcaoEnum Acao { get; set; }

    [Required]
    [Column("data", TypeName = "timestamp without time zone")]
    public DateTime Data { get; set; }

    [Required]
    [Column("origem")]
    public AcLogOrigemEnum Origem { get; set; }
}
