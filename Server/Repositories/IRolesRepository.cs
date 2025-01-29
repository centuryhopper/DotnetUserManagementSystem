
using Microsoft.AspNetCore.Identity;
using Server.Entities;
using Shared.Models;
using static Shared.Models.ServiceResponses;


public interface IRolesRepository
{
    Task<IEnumerable<RoleDTO>> GetRolesAsync();
    Task<GeneralResponse> AddRoleAsync(RoleDTO dto);
    Task<IEnumerable<GeneralResponse>> AddRolesAsync(IEnumerable<RoleDTO> dtos);
    Task<GeneralResponse> EditRoleAsync(RoleDTO dto);
    Task<GeneralResponse> DeleteRoleAsync(string roleId);
}