using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Contexts;
using Server.Entities;


namespace Server.Repositories;

public class RolesRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : IRolesRepository
{
    public async Task<IEnumerable<HandyGeneralResponse>> AddRolesAsync(IEnumerable<RoleDTO> dtos)
    {
        List<HandyGeneralResponse> responses = [];
        foreach (var dto in dtos)
        {
            responses.Add(await AddRoleAsync(dto));
        }
        return responses;
    }

    public async Task<HandyGeneralResponse> AddRoleAsync(RoleDTO dto)
    {
        if (await roleManager.RoleExistsAsync(dto.RoleName))
        {
            return new HandyGeneralResponse(Flag: false, Message: $"role {dto.RoleName} already exists");
        }

        var result = await roleManager.CreateAsync(new ApplicationRole
        {
            Name = dto.RoleName,
            IsActive = dto.IsActive,
        });

        if (!result.Succeeded)
        {
            return new HandyGeneralResponse(Flag: false, Message: string.Join("$$$", result.Errors.Select(e => e.Description)));
        }

        return new HandyGeneralResponse(Flag: true, Message: $"{dto.RoleName} role has been created.");
    }

    public async Task<HandyGeneralResponse> DeleteRoleAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        var result = await roleManager.DeleteAsync(role!);

        if (!result.Succeeded)
        {
            return new HandyGeneralResponse(Flag: false, Message: "failed to delete role");
        }

        return new HandyGeneralResponse(Flag: true, Message: "role deleted!");
    }

    public async Task<HandyGeneralResponse> EditRoleAsync(RoleDTO dto)
    {
        var role = await roleManager.FindByIdAsync(dto.Id);
        role.Name = dto.RoleName;
        role.IsActive = dto.IsActive;

        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return new HandyGeneralResponse(Flag: false, Message: "failed to edit role");
        }

        return new HandyGeneralResponse(Flag: true, Message: "role edited!");

    }

    public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
    {
        return await roleManager.Roles.Select(r => new RoleDTO
        {
            Id = r.Id,
            RoleName = r.Name,
            IsActive = r.IsActive,
        }).ToListAsync();
    }
}

