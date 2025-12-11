using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Server.Entities;
using Server.Utils;
using Shared.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UMSController(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IApplicationsRepository applicationsRepository,
    IWebHostEnvironment env,
    IConfiguration configuration) : ControllerBase
{
    // POST: api/UMS/get-user-credentials
    [HttpPost("get-user-credentials")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> GetUserCredentialsAsync([FromBody] LoginDTO dto, string appName)
    {
        var getUser = await userManager.FindByEmailAsync(dto.Email);
        if (getUser is null)
        {
            return BadRequest("The user with this email was not found in the UMS.");
        }

        bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, dto.Password);
        if (!checkUserPasswords)
        {
            return BadRequest("Invalid email/password");
        }

        // grab records with the application name of "appName" query parameter
        // that belong to the current user
        var apps = await applicationsRepository.GetApplicationsByAppNameAndUserIdAsync(appName, getUser.Id);

        // App-specific 2FA check
        var anApp = apps.First(); // assuming one app per appName per user
        if (anApp.RequiresTwoFactor && getUser.TwoFactorEnabled)
        {
            // Send 2FA code (you could send via SMS, Email, or use authenticator apps)
            var code = await userManager.GenerateTwoFactorTokenAsync(getUser, TokenOptions.DefaultEmailProvider);

            // NOTE: Send the token to the user via email or other means
            // await emailSender.SendAsync(user.Email, "Your 2FA Code", $"Code: {code}");
            var smtpInfo = env.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

            // var key = await userManager.GetAuthenticatorKeyAsync(getUser);

            var mfaCodeLink = (env.IsDevelopment() ? configuration.GetConnectionString("BaseUrl") : Environment.GetEnvironmentVariable("BaseUrl")) + $"/verify-2fa?userId={getUser.Id}&code={code}";

            Helpers.SendEmail(subject: "Your 2FA Confirm Code", senderEmail: smtpInfo[0], senderPassword: smtpInfo[1], body: mfaCodeLink, receivers: [dto.Email]);

            return Ok(new
            {
                requiresTwoFactor = true,
                message = "2FA code sent to your email.",
            });
        }

        // join them with roles table and get the role names
        var roles = roleManager.Roles.AsEnumerable();

        var getRoles = from app in apps
                       join role in roles on app.Roleid equals role.Id
                       select new
                       {
                           Role = role.Name,
                       };


        return Ok(new
        {
            username = getUser.UserName,
            email = getUser.Email,
            Roles = getRoles,
            userId = getUser.Id,
        });
    }

    private async Task<GeneralResponse> HandleMFA(string email, string appName)
    {
        var user = await userManager.FindByEmailAsync(email);
        var apps = await applicationsRepository.GetApplicationsByAppNameAndUserIdAsync(appName, user.Id);

        // App-specific 2FA check
        var anApp = apps.First(); // assuming one app per appName per user

        // TODO: enable identity user two factor auth in the user controller
        if (anApp.RequiresTwoFactor && user.TwoFactorEnabled)
        {
            // Send 2FA code (you could send via SMS, Email, or use authenticator apps)
            var code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            // NOTE: Send the token to the user via email or other means
            // await emailSender.SendAsync(user.Email, "Your 2FA Code", $"Code: {code}");
            var smtpInfo = env.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

            var mfaCodeLink = (env.IsDevelopment() ? configuration.GetConnectionString("BaseUrl") : Environment.GetEnvironmentVariable("BaseUrl")) + $"/verify-2fa?userId={user.Id}&code={code}";

            Helpers.SendEmail(subject: "Your 2FA Confirm Code", senderEmail: smtpInfo[0], senderPassword: smtpInfo[1], body: mfaCodeLink, receivers: [email]);


            // return Ok(new
            // {
            //     requiresTwoFactor = true,
            //     message = "2FA code sent to your email.",
            // });
        }
    }


    [HttpGet("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromQuery] string userId, [FromQuery] string code)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return BadRequest("User not found.");
        }

        bool isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultEmailProvider,
            code
        );

        if (!isValid)
            return BadRequest("Invalid 2FA code.");

        // Success – return user info
        return Ok(new
        {
            username = user.UserName,
            email = user.Email,
            userId = user.Id,
            message = "2FA verification successful"
        });
    }

    [HttpPost("set-2fa")]
    public async Task<IActionResult> SetAppTwoFactor([FromBody] App2FASettingsDTO dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        var app = (await applicationsRepository.GetApplicationsByAppNameAndUserIdAsync(dto.AppName, user.Id)).FirstOrDefault();

        if (app == null)
        {
            return NotFound("Application not found.");
        }

        app.RequiresTwoFactor = dto.Enable2FA;
        await applicationsRepository.EditApplicationAsync(app);

        return Ok(new { message = $"2FA {(dto.Enable2FA ? "enabled" : "disabled")} for app." });
    }

}
