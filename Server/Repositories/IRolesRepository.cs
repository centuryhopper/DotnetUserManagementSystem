
using Microsoft.AspNetCore.Identity;
using Server.Entities;
using Shared.Models;



public interface IRolesRepository
{
    Task<IEnumerable<RoleDTO>> GetRolesAsync();
    Task<HandyGeneralResponse> AddRoleAsync(RoleDTO dto);
    Task<IEnumerable<HandyGeneralResponse>> AddRolesAsync(IEnumerable<RoleDTO> dtos);
    Task<HandyGeneralResponse> EditRoleAsync(RoleDTO dto);
    Task<HandyGeneralResponse> DeleteRoleAsync(string roleId);
}