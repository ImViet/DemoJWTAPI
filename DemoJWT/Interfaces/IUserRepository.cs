using DemoShared.DTO;

namespace DemoJWT.Interfaces
{
    public interface IUserRepository
    {
        Task<string> Authenticate(LoginRequestDTO request);
        Task<bool> Register(RegisterRequestDTO request);
    }
}
