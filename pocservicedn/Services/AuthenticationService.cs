using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using pocservicedn.Contracts;
using pocservicedn.Entities;
using pocservicedn.Enums;
using pocservicedn.Repositories;

namespace pocservicedn.Services;

public sealed class AuthenticationService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher<AcUsuarioEntity> passwordHasher,
    IJwtTokenService jwtTokenService,
    ILogger<AuthenticationService> logger) : IAuthenticationService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.ObterPorLoginAsync(request.Login, cancellationToken);
        if (usuario is null)
        {
            return null;
        }

        var verificationResult = VerificarSenhaArmazenada(usuario, request.Senha);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.Senha = passwordHasher.HashPassword(usuario, request.Senha);
        }

        await usuarioRepository.AdicionarLogAsync(
            new AcLogEntity
            {
                IdUsuario = usuario.Id,
                Acao = AcLogAcaoEnum.Login,
                Origem = request.Origem
            },
            cancellationToken);

        return jwtTokenService.CreateToken(usuario);
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(
        int authenticatedUserId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.ObterPorIdAsync(authenticatedUserId, cancellationToken);
        if (usuario is null)
        {
            return ChangePasswordResult.UserNotFound;
        }

        if (!string.Equals(usuario.Login, request.Login, StringComparison.Ordinal))
        {
            return ChangePasswordResult.LoginMismatch;
        }

        if (VerificarSenhaArmazenada(usuario, request.SenhaAtual) == PasswordVerificationResult.Failed)
        {
            return ChangePasswordResult.InvalidCurrentPassword;
        }

        usuario.Senha = passwordHasher.HashPassword(usuario, request.NovaSenha);
        await usuarioRepository.SalvarAlteracoesAsync(cancellationToken);
        return ChangePasswordResult.Success;
    }

    public async Task<UserProfileResponse?> ObterPerfilAsync(
        int authenticatedUserId,
        CancellationToken cancellationToken)
    {
        var dados = await usuarioRepository.ObterPorIdComPessoaAsync(authenticatedUserId, cancellationToken);
        return dados is null
            ? null
            : new UserProfileResponse(
                dados.Value.Usuario.Id,
                dados.Value.Usuario.Login,
                new PessoaResponse(
                    dados.Value.Pessoa.Id,
                    dados.Value.Pessoa.Nome,
                    dados.Value.Pessoa.Sobrenome,
                    dados.Value.Pessoa.DataNascimento,
                    dados.Value.Pessoa.Email,
                    dados.Value.Pessoa.Telefone,
                    dados.Value.Pessoa.Endereco));
    }

    private PasswordVerificationResult VerificarSenhaArmazenada(
        AcUsuarioEntity usuario,
        string senhaInformada)
    {
        try
        {
            return passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senhaInformada);
        }
        catch (FormatException exception)
        {
            logger.LogError(
                exception,
                "The stored password hash for user {UserId} is not valid Base64.",
                usuario.Id);
            return PasswordVerificationResult.Failed;
        }
    }
}
