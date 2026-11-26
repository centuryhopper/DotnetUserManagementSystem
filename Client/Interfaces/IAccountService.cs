
using Shared.Models;
using static Shared.Models.ServiceResponses;


namespace Client.Interfaces;

public interface IAccountService
{
    Task<GeneralResponse> ResetPasswordAsync(ResetPasswordDTO dto);
    Task<GeneralResponse> EditProfileAsync(ProfileDTO dto);
    Task<LoginResponse> LoginAsync(LoginDTO dto);
    Task<GeneralResponse> RegisterAsync(RegisterDTO dto);
    Task<GeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto);
    Task LogoutAsync();
}


