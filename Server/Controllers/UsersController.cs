using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Controllers;

[Authorize(Roles = Constants.ADMIN)]
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


    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers()
    {
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
