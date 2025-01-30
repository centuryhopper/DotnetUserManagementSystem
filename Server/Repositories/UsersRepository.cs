
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Contexts;
using Server.Entities;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public class UsersRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IUsersRepository
{
    public async Task<UserDTO> GetUserByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        return new UserDTO
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Roles = (await userManager.GetRolesAsync(user)).ToList(),
        };
    }

    public async Task<UserDTO> GetUserByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        return new UserDTO
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Roles = (await userManager.GetRolesAsync(user)).ToList(),
        };
    }

    public async Task<UserDTO> GetUserByUsernameAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);

        return new UserDTO
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Roles = (await userManager.GetRolesAsync(user)).ToList(),
        };
    }

    public async Task<GeneralResponse> VerifyPassword(UserDTO dto, string password)
    {
        var user = await userManager.FindByIdAsync(dto.Id);
        user ??= await userManager.FindByEmailAsync(dto.Email);
        user ??= await userManager.FindByNameAsync(dto.Username);

        if (user is null)
        {
            return new GeneralResponse(false, "User was not found");
        }

        var check = await userManager.CheckPasswordAsync(user, password);

        return new GeneralResponse(check, check ? "Success" : "Fail");
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        var users = await userManager.Users.ToListAsync();
        Dictionary<string, List<string>> userRoles = new();

        foreach (var user in users)
        {
            userRoles.Add(user.Id, (await userManager.GetRolesAsync(user)).ToList());
        }

        return users.Select(u => new UserDTO
        {
            Id = u.Id,
            Username = u.UserName,
            Email = u.Email,
            Roles = userRoles[u.Id],
        }).ToList();
    }
}

