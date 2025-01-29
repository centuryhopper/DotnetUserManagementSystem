
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Client.Interfaces;

public interface IAccountService
{
    Task<LoginResponse> LoginAsync(LoginDTO dto);
    Task<GeneralResponse> RegisterAsync(RegisterDTO dto);
    Task<GeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto);
    Task LogoutAsync();
}


