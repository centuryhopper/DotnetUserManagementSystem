using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models;


namespace Server.Controllers;

[Authorize(Roles = Shared.Constants.ADMIN)]
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRolesRepository rolesRepository, ILogger<RolesController> logger) : ControllerBase
{
    [HttpGet("nlog-test")]
    public IActionResult Test()
    {
        logger.LogWarning("warning!");
        logger.LogInformation("information!");
        logger.LogError("error!");
        logger.LogCritical("critical");
        logger.LogDebug("debug");
        return Ok("Logging test completed. Check your PostgreSQL LOGS table.");
    }

    [HttpGet("get-roles")]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var response = await rolesRepository.GetRolesAsync();
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpPost("add-roles")]
    public async Task<IActionResult> AddRoles([FromBody] IEnumerable<RoleDTO> dtos)
    {
        try
        {
            var response = await rolesRepository.AddRolesAsync(dtos);
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] RoleDTO dto)
    {
        try
        {
            var response = await rolesRepository.AddRoleAsync(dto);
            if (!response.Flag)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpPatch("edit-role")]
    public async Task<IActionResult> EditRole([FromBody] RoleDTO dto)
    {
        try
        {
            var response = await rolesRepository.EditRoleAsync(dto);
            if (!response.Flag)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpDelete("delete-role/{roleId}")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        try
        {
            var response = await rolesRepository.DeleteRoleAsync(roleId);
            if (!response.Flag)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }


}
