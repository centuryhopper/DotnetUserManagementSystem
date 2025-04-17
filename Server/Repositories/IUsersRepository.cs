
using Microsoft.AspNetCore.Identity;
using Server.Entities;
using Shared.Models;



public interface IUsersRepository
{
    Task<HandyGeneralResponse> VerifyPassword(UserDTO dto, string password);
    Task<UserDTO> GetUserByIdAsync(string id);
    Task<UserDTO> GetUserByEmailAsync(string email);
    Task<UserDTO> GetUserByUsernameAsync(string username);
    Task<IEnumerable<UserDTO>> GetUsersAsync();
}