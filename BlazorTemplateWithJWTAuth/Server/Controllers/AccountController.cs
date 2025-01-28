using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Shared.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager) : ControllerBase
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

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var response = await accountRepository.LoginAsync(dto);
        //logger.LogInformation(response.Message);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO dto)
    {
        var response = await accountRepository.ForgotPasswordAsync(dto);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var response = await accountRepository.RegisterAsync(dto);
        return Ok(response);
    }


}
