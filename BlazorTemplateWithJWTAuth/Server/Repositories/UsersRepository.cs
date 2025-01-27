
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
    public async Task<IEnumerable<UserDTO>> GetUsersAsync(UserDTO dto)
    {
        var users = await userManager.Users.ToListAsync();
        Dictionary<string, List<string>> userRoles = new();

        foreach (var user in users)
        {
            userRoles.Add(user.Id, (await userManager.GetRolesAsync(user)).ToList());
        }

        return users.Select(u => new UserDTO {
            Id = u.Id,
            Username = u.UserName,
            Email = u.Email,
            Roles = userRoles[u.Id],
        }).ToList();
    }
}

