using Microsoft.EntityFrameworkCore;
using pocservicedn.Data;
using pocservicedn.Entities;

namespace pocservicedn.Repositories;

public sealed class UsuarioRepository(ApplicationDbContext dbContext) : IUsuarioRepository
{
    public Task<AcUsuarioEntity?> ObterPorLoginAsync(string login, CancellationToken cancellationToken) =>
        dbContext.Usuarios
            .SingleOrDefaultAsync(usuario => usuario.Login == login, cancellationToken);

    public Task<AcUsuarioEntity?> ObterPorIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Usuarios
            .SingleOrDefaultAsync(usuario => usuario.Id == id, cancellationToken);

    public async Task<(AcUsuarioEntity Usuario, SiPessoaEntity Pessoa)?> ObterPorIdComPessoaAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var resultado = await (
            from usuario in dbContext.Usuarios.AsNoTracking()
            join pessoa in dbContext.Pessoas.AsNoTracking()
              on usuario.IdPessoa equals pessoa.Id
            where usuario.Id == id
            select new { Usuario = usuario, Pessoa = pessoa })
            .SingleOrDefaultAsync(cancellationToken);

        return resultado is null ? null : (resultado.Usuario, resultado.Pessoa);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public async Task AdicionarLogAsync(AcLogEntity log, CancellationToken cancellationToken)
    {
        dbContext.Logs.Add(log);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
