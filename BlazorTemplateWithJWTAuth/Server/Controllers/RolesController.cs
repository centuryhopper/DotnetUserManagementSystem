using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models;

namespace Server.Controllers;

[Authorize(Roles = Constants.ADMIN)]
[Route("api/[controller]")]
[ApiController]
public class RolesController(IAccountRepository accountRepository, ILogger<RolesController> logger) : ControllerBase
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
        return Ok();
    }

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] RoleDTO dto)
    {
        return Ok();
    }

    [HttpPut("edit-role")]
    public async Task<IActionResult> EditRole([FromBody] RoleDTO dto)
    {
        return Ok();
    }

    [HttpDelete("delete-role/{roleId}")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        return Ok();
    }

   
}
