using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocservicedn.Entities;

[Table("si_pessoa", Schema = "public")]
public sealed class SiPessoaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    [Column("nome")]
    public required string Nome { get; set; }

    [Required, MaxLength(100)]
    [Column("sobrenome")]
    public required string Sobrenome { get; set; }

    [Column("data_nascimento")]
    public DateOnly? DataNascimento { get; set; }

    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(15)]
    [Column("telefone")]
    public string? Telefone { get; set; }

    [MaxLength(255)]
    [Column("endereco")]
    public string? Endereco { get; set; }
}
