using pocservicedn.Entities;
using pocservicedn.Contracts;

namespace pocservicedn.Services;

public interface IJwtTokenService
{
    LoginResponse CreateToken(AcUsuarioEntity usuario);
}
