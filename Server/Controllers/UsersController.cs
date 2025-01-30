using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Controllers;


[Authorize(Roles = Shared.Constants.ADMIN)]
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUsersRepository usersRepository, ILogger<UsersController> logger) : ControllerBase
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

    [HttpGet("get-claims")]
    public IActionResult Claims()
    {
        var user = User;

        return Ok(user.Claims.Select(c => new {
            key = c.Type,
            value = c.Value,
        }));
    }



    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var user = User;
        try
        {
            var response = await usersRepository.GetUsersAsync();
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

}
