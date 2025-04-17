

using Microsoft.AspNetCore.Identity;
using Shared.Models;



public interface IAccountRepository
{
    Task<LoginResponse> LoginAsync(LoginDTO dto);
    Task<GeneralResponse> EditProfileAsync(ProfileDTO dto);
    Task<GeneralResponse> RegisterAsync(RegisterDTO dto);
    Task<GeneralResponse> ResetPasswordAsync(ResetPasswordDTO dto);
    Task<GeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto);
    Task<GeneralResponse> ConfirmEmailAsync(ConfirmEmailDTO dto);
    Task<GeneralResponse> CreateRole(string roleName);
}