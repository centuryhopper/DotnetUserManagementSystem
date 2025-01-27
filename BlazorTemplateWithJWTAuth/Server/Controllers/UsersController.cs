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
public class UsersController(IAccountRepository accountRepository, ILogger<UsersController> logger) : ControllerBase
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

    public async Task<IActionResult> GetUsers()
    {
        return Ok();
    }

   
}
