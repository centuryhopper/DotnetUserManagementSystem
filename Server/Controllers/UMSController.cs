using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using Server.Entities;
using Server.Services;
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
    IConfiguration configuration,
    IEmailService emailService) : ControllerBase
{
    [HttpGet("resend-test")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> ResendTestAsync()
    {
        await emailService.SendEmailAsync(
            toEmail: "leotheasianlion@gmail.com",
            subject: "Test Email from Resend",
            body: "This is a test email sent using the Resend service."
        );

        return Ok(new GeneralResponse(true, "Email sent successfully."));
    }

    [HttpGet("check-user/{email}/{pwd}")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> CheckUserExistsAsync(string email, string pwd)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return NotFound(new GeneralResponse(false, "User not found."));
        }

        bool isPasswordValid = await userManager.CheckPasswordAsync(user, pwd);
        if (!isPasswordValid)
        {
            return BadRequest(new GeneralResponse(false, "Invalid password."));
        }

        return Ok(new GeneralResponse(true, "User exists and password is valid."));
    }

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
        if (/*anApp.RequiresTwoFactor &&*/ getUser.TwoFactorEnabled)
        {
            // Send 2FA code (you could send via SMS, Email, or use authenticator apps)
            // var code = await userManager.GenerateTwoFactorTokenAsync(getUser, TokenOptions.DefaultEmailProvider);

            // NOTE: Send the token to the user via email or other means
            // await emailSender.SendAsync(user.Email, "Your 2FA Code", $"Code: {code}");
            // var smtpInfo = env.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

            // var key = await userManager.GetAuthenticatorKeyAsync(getUser);

            // TODO: create an endpoint link for verifying 2FA codes on an application level
            // var mfaCodeLink = (env.IsDevelopment() ? configuration.GetConnectionString("BaseUrl") : Environment.GetEnvironmentVariable("BaseUrl")) + $"/verify-2fa?email={getUser.Email}&code={code}";

            // Helpers.SendEmail(subject: "Your 2FA Confirm Code", senderEmail: smtpInfo[0], senderPassword: smtpInfo[1], body: mfaCodeLink, receivers: [dto.Email]);

            return Ok(new
            {
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

    [HttpGet("get-2fa-status/{email}")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> GetTwoFactorStatus(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return BadRequest("User not found.");
        }

        return Ok(new { twoFactorEnabled = user.TwoFactorEnabled });
    }

    /*
        an endpoint for turning on/off 2FA
        POST: api/UMS/set-2fa        
    */

    [HttpPost("set-2fa/{email}/{enable2FA}")]
    [EnableRateLimiting("FixedPolicy")]
    [Authorize]
    public async Task<IActionResult> SetTwoFactorAsync(string email, bool enable2FA)
    {
        // Only the ums can call this endpoint
        var host = Request.Host.Value!.ToLower();

        // System.Console.WriteLine("Request Host: " + host);
        if (host.Contains("localhost") || host.Contains("https://dotnetusermanagementsystem-production.up.railway.app/"))
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var result = await userManager.SetTwoFactorEnabledAsync(user, enable2FA);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to update 2FA status." });
            }

            return Ok(new { message = $"2FA {(enable2FA ? "enabled" : "disabled")} for user." });
        }
        else
        {
            return BadRequest("Unauthorized request.");
        }
    }

    [HttpGet("verify-2fa")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromQuery] string email, [FromQuery] string code)
    {
        var user = await userManager.FindByEmailAsync(email);
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
        {
            return BadRequest("Invalid 2FA code.");
        }

        var userRoles = await userManager.GetRolesAsync(user);

        // Success – return user info
        return Ok(new
        {
            userId = user.Id,
            username = user.UserName,
            email = user.Email,
            userRoles,
            message = "2FA verification successful",
            jwtToken = GenerateToken(user.Id, user.UserName, user.Email, userRoles.First())
        });
    }

    [HttpGet("send-2fa-code/{email}")]
    [EnableRateLimiting("FixedPolicy")]
    public async Task<IActionResult> SendTwoFactorCodeAsync(string email)
    {
        var getUser = await userManager.FindByEmailAsync(email);
        if (getUser is null)
        {
            return BadRequest(new { message = "User not found.", flag = false });
        }

        if (getUser.TwoFactorEnabled)
        {
            var twoFactorToken = await userManager.GenerateTwoFactorTokenAsync(
                getUser,
                TokenOptions.DefaultEmailProvider
            );

            await emailService.SendEmailAsync(
                toEmail: email,
                subject: "2FA Verification",
                body: Helpers.Build2FAHtmlEmail(getUser, twoFactorToken)
            );

            return Ok(new { message = twoFactorToken, flag = true });
        }

        return BadRequest(new { message = "2FA is not enabled for this user.", flag = false });
    }

    private string GenerateToken(string userId, string userName, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(env.IsDevelopment() ? configuration["Jwt:Key"] : Environment.GetEnvironmentVariable("Jwt_Key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: env.IsDevelopment() ? configuration["Jwt:Issuer"] : Environment.GetEnvironmentVariable("Jwt_Issuer"),
            audience: env.IsDevelopment() ? configuration["Jwt:Audience"] : Environment.GetEnvironmentVariable("Jwt_Audience"),
            claims: userClaims,
            expires: JwtConfig.JWT_TOKEN_EXP_DATETIME,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
