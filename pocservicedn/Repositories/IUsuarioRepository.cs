using pocservicedn.Entities;

namespace pocservicedn.Repositories;

public interface IUsuarioRepository
{
    Task<AcUsuarioEntity?> ObterPorLoginAsync(string login, CancellationToken cancellationToken);
    Task<AcUsuarioEntity?> ObterPorIdAsync(int id, CancellationToken cancellationToken);
    Task<(AcUsuarioEntity Usuario, SiPessoaEntity Pessoa)?> ObterPorIdComPessoaAsync(
        int id,
        CancellationToken cancellationToken);
    Task AdicionarLogAsync(AcLogEntity log, CancellationToken cancellationToken);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken);
}
