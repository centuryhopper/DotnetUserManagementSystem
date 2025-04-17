using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shared;
using Shared.Models;


namespace Server.Controllers;



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

    // [HttpGet("get-claims")]
    // public IActionResult Claims()
    // {
    //     var user = User;

    //     return Ok(user.Claims.Select(c => new {
    //         key = c.Type,
    //         value = c.Value,
    //     }));
    // }

    [HttpPost("check-password/{password}")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> VerifyPassword(string password, [FromBody] UserDTO dto)
    {
        try
        {
            var response = await usersRepository.VerifyPassword(dto, password);
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new HandyGeneralResponse(false, ex.Message));
        }
    }

    [HttpGet("get-users-by-username/{username}")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        try
        {
            var response = await usersRepository.GetUserByUsernameAsync(username);
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new HandyGeneralResponse(false, ex.Message));
        }
    }

    [HttpGet("get-users-by-email/{email}")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        try
        {
            var response = await usersRepository.GetUserByEmailAsync(email);
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new HandyGeneralResponse(false, ex.Message));
        }
    }

    [HttpGet("get-users-by-id/{id}")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var response = await usersRepository.GetUserByIdAsync(id);
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new HandyGeneralResponse(false, ex.Message));
        }
    }

    [Authorize(Roles = Shared.Constants.ADMIN)]
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
            return BadRequest(new HandyGeneralResponse(false, ex.Message));
        }
    }

}
