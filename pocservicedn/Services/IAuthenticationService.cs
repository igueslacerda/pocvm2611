using pocservicedn.Contracts;

namespace pocservicedn.Services;

public interface IAuthenticationService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<ChangePasswordResult> ChangePasswordAsync(
        int authenticatedUserId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken);
    Task<UserProfileResponse?> ObterPerfilAsync(int authenticatedUserId, CancellationToken cancellationToken);
}

public enum ChangePasswordResult
{
    Success,
    UserNotFound,
    LoginMismatch,
    InvalidCurrentPassword
}
