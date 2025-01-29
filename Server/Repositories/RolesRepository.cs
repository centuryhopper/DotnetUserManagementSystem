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

public class RolesRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : IRolesRepository
{
    public async Task<IEnumerable<GeneralResponse>> AddRolesAsync(IEnumerable<RoleDTO> dtos)
    {
        List<GeneralResponse> responses = [];
        foreach (var dto in dtos)
        {
            responses.Add(await AddRoleAsync(dto));
        }
        return responses;
    }

    public async Task<GeneralResponse> AddRoleAsync(RoleDTO dto)
    {
        if (await roleManager.RoleExistsAsync(dto.RoleName))
        {
            return new GeneralResponse(Flag: false, Message: $"role {dto.RoleName} already exists");
        }

        var result = await roleManager.CreateAsync(new ApplicationRole {
            Name = dto.RoleName,
            IsActive = dto.IsActive,
        });

        if (!result.Succeeded)
        {
            return new GeneralResponse(Flag: false, Message: string.Join("$$$", result.Errors.Select(e=>e.Description)));
        }

        return new GeneralResponse(Flag: true, Message: $"{dto.RoleName} role has been created.");
    }

    public async Task<GeneralResponse> DeleteRoleAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        var result = await roleManager.DeleteAsync(role!);

        if (!result.Succeeded)
        {
            return new GeneralResponse(Flag: false, Message: "failed to delete role");
        }

        return new GeneralResponse(Flag: true, Message: "role deleted!");
    }

    public async Task<GeneralResponse> EditRoleAsync(RoleDTO dto)
    {
        var role = await roleManager.FindByIdAsync(dto.Id);
        role.Name = dto.RoleName;
        role.IsActive = dto.IsActive;
        
        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return new GeneralResponse(Flag: false, Message: "failed to edit role");
        }

        return new GeneralResponse(Flag: true, Message: "role edited!");

    }

    public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
    {
        return await roleManager.Roles.Select(r => new RoleDTO {
            Id = r.Id,
            RoleName = r.Name,
            IsActive = r.IsActive,
        }).ToListAsync();
    }
}

