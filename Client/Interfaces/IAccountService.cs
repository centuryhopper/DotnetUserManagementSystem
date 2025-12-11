
using Shared.Models;


namespace Client.Interfaces;

public interface IAccountService
{
    Task<HandyGeneralResponse> ResetPasswordAsync(ResetPasswordDTO dto);
    Task<HandyGeneralResponse> EditProfileAsync(ProfileDTO dto);
    Task<HandyLoginResponse> LoginAsync(LoginDTO dto);
    Task<HandyGeneralResponse> RegisterAsync(RegisterDTO dto);
    Task<HandyGeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto);
    Task LogoutAsync();
}


