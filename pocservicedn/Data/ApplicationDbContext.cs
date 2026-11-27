using Microsoft.EntityFrameworkCore;
using pocservicedn.Entities;

namespace pocservicedn.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<SiPessoaEntity> Pessoas => Set<SiPessoaEntity>();
    public DbSet<AcUsuarioEntity> Usuarios => Set<AcUsuarioEntity>();
    public DbSet<AcLogEntity> Logs => Set<AcLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcUsuarioEntity>(entity =>
        {
            entity.HasAlternateKey(usuario => usuario.Login).HasName("ac_usuario_login_key");
            entity.Property(usuario => usuario.Id).UseIdentityAlwaysColumn();
            entity.HasOne<SiPessoaEntity>()
                .WithMany()
                .HasForeignKey(usuario => usuario.IdPessoa)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("ac_usuario_id_pessoa_fkey");
        });

        modelBuilder.Entity<AcLogEntity>(entity =>
        {
            entity.Property(log => log.Id).UseIdentityAlwaysColumn();
            entity.Property(log => log.Acao).HasConversion<int>();
            entity.Property(log => log.Data).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(log => log.Origem).HasConversion<int>();
            entity.HasOne<AcUsuarioEntity>()
                .WithMany()
                .HasForeignKey(log => log.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("ac_log_id_usuario_fkey");
        });

        modelBuilder.Entity<SiPessoaEntity>()
            .Property(pessoa => pessoa.Id)
            .UseIdentityAlwaysColumn();
    }
}
