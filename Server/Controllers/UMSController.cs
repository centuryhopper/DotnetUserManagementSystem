using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Server.Entities;
using Shared.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UMSController(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IApplicationsRepository applicationsRepository) : ControllerBase
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

        // join them with roles table and get the role names
        var roles = roleManager.Roles.AsEnumerable();

        var getRoles = from app in apps
                        join role in roles on app.Roleid equals role.Id
                        select new {
                            Role = role.Name,
                        };


        return Ok(new {
            username = getUser.UserName,
            email = getUser.Email,
            Roles = getRoles,
            userId = getUser.Id,
        });
    }
}
