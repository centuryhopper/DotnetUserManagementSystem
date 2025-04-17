

using Microsoft.AspNetCore.Identity;
using Shared.Models;



public interface IAccountRepository
{
    Task<HandyLoginResponse> LoginAsync(LoginDTO dto);
    Task<HandyGeneralResponse> EditProfileAsync(ProfileDTO dto);
    Task<HandyGeneralResponse> RegisterAsync(RegisterDTO dto);
    Task<HandyGeneralResponse> ResetPasswordAsync(ResetPasswordDTO dto);
    Task<HandyGeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto);
    Task<HandyGeneralResponse> ConfirmEmailAsync(ConfirmEmailDTO dto);
    Task<HandyGeneralResponse> CreateRole(string roleName);
}