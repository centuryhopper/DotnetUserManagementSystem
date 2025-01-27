

using Microsoft.AspNetCore.Identity;
using Shared.Models;
using static Shared.Models.ServiceResponses;


public interface IAccountRepository
{
    Task<LoginResponse> LoginAsync(LoginDTO dto);
    Task<GeneralResponse> EditProfileAsync(ProfileDTO dto, string confirmationLink);
    Task<GeneralResponse> RegisterAsync(RegisterDTO dto, string confirmationLink);
    Task<GeneralResponse> ResetPasswordAsync(ResetPasswordDTO dto);
    Task<GeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto, string passwordResetLink);
    Task<GeneralResponse> ConfirmEmailAsync(ConfirmEmailDTO dto);
    Task<GeneralResponse> CreateRole(string roleName);
}