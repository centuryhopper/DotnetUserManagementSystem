
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Contexts;
using Server.Entities;
using static Shared.Models.ServiceResponses;
using Server.Utils;
using Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace Server.Repositories;

public class AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : IAccountRepository
{
    public async Task<GeneralResponse> ConfirmEmailAsync(ConfirmEmailDTO dto)
    {
        if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrEmpty(dto.UserId))
        {
            return new GeneralResponse(Flag: false, Message: "Missing user id or token.");
        }

        var user = await userManager.FindByIdAsync(dto.UserId);

        if (user is null)
        {
            return new GeneralResponse(Flag: false, Message: "The user id is invalid");
        }

        var result = await userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token)));

        if (result.Succeeded)
        {
            // username field will be the backup in case user couldn't confirm the new email.
            // i.e. the username field will be their last resort way of logging back in and confirming a new email
            user.UserName = user.Email;
            user.NormalizedUserName = user.UserName.ToUpper();
            await userManager.UpdateAsync(user);
            return new GeneralResponse(Flag: true, Message: "The email has been successfully confirmed!");
        }

        // TempData[TempDataKeys.ALERT_ERROR] = "We couldn't confirm your email.";
        return new GeneralResponse(Flag: false, Message: string.Join("$$$", result.Errors.Select(e => e.Description)));
    }

    public async Task<GeneralResponse> CreateRole(string roleName)
    {
        // We just need to specify a unique role name to create a new role
        ApplicationRole role = new ApplicationRole
        {
            Name = roleName
        };

        // Saves the role in the underlying AspNetRoles table
        IdentityResult result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            return new GeneralResponse(Flag: false, Message: string.Join("$$$", result.Errors.Select(e => e.Description)));
        }

        return new GeneralResponse(Flag: true, Message: "role created!");
    }

    public async Task<GeneralResponse> EditProfileAsync(ProfileDTO dto)
    {
        var user = await userManager.FindByIdAsync(dto.Id);
        if (user == null)
        {
            return new GeneralResponse(Flag: false, Message: "user is false");
        }

        List<string> errors = [];
        List<string> successes = [];

        if (!string.IsNullOrEmpty(dto.CurrentEmail) && dto.CurrentEmail.ToLower() != user.Email!.ToLower())
        {
            // Change the email
            var token = await userManager.GenerateChangeEmailTokenAsync(user, dto.CurrentEmail);
            var result = await userManager.ChangeEmailAsync(user, dto.CurrentEmail, token);

            bool emailChanged = false;

            if (result.Succeeded)
            {
                if (userManager.Options.SignIn.RequireConfirmedEmail)
                {
                    var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

                    var confirmationLink = (webHostEnvironment.IsDevelopment() ? configuration.GetConnectionString("BaseUrl") : Environment.GetEnvironmentVariable("BaseUrl")) + $"/confirm-email?userId={user.Id}&token={encodedToken}";

                    var smtpInfo = webHostEnvironment.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

                    Helpers.SendEmail(subject: "confirm email", senderEmail: smtpInfo[0], senderPassword: smtpInfo[1], body: confirmationLink, receivers: [dto.CurrentEmail]);

                    successes.Add("Email updated! Please confirm it to login.");
                }
                else
                {
                    emailChanged = true;
                    successes.Add("Email updated!");
                }
            }
            else
            {
                errors.Add("Couldn't update your email.");
            }

            if (emailChanged)
            {
                // username field will be the backup in case user couldn't confirm the new email.
                // i.e. the username field will be their last resort way of logging back in and confirming a new email
                user.UserName = user.Email;
                user.NormalizedUserName = user.UserName.ToUpper();
                await userManager.UpdateAsync(user);
            }
        }



        if (!string.IsNullOrEmpty(dto.NewPassword))
        {
            // ChangePasswordAsync changes the user password
            var result = await userManager.ChangePasswordAsync(user,
                dto.CurrentPassword, dto.NewPassword);

            // The new password did not meet the complexity rules or
            // the current password is incorrect. Add these errors to
            // the ModelState and rerender ChangePassword view
            if (!result.Succeeded)
            {
                return new GeneralResponse(Flag: false, Message: string.Join("$$$", result.Errors.Select(e => e.Description)));
            }

            // Upon successfully changing the password refresh sign-in cookie
            await signInManager.RefreshSignInAsync(user);

            successes.Add("We have successfully changed your password!");
        }

        if (successes.Any())
        {
            return new GeneralResponse(Flag: true, Message: string.Join("$$$", successes));

        }
        if (errors.Any())
        {
            return new GeneralResponse(Flag: false, Message: string.Join("$$$", errors));
        }

        return new GeneralResponse(Flag: true, Message: "no changes made");
    }

    public async Task<GeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto)
    {
        // Find the user by email
        var user = await userManager.FindByEmailAsync(dto.Email);

        // If the user is found AND Email is confirmed
        if (user != null && await userManager.IsEmailConfirmedAsync(user))
        {
            // Generate the reset password token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var passwordResetLink = (webHostEnvironment.IsDevelopment() ? configuration.GetConnectionString("BaseUrl") : Environment.GetEnvironmentVariable("BaseUrl")) + $"/reset-password?email={user.Email}&token={encodedToken}";

            var smtpInfo = webHostEnvironment.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

            Helpers.SendEmail(subject: "password reset link", senderEmail: smtpInfo[0], senderPassword: smtpInfo[1], body: passwordResetLink, receivers: [dto.Email]);

            return new GeneralResponse(Flag: true, Message: "If you have an account with us, we have sent an email with the instructions to reset your password.");
        }

        // same return value in order to prevent cyber attacks
        return new GeneralResponse(Flag: true, Message: "If you have an account with us, we have sent an email with the instructions to reset your password.");
    }

    public async Task<LoginResponse> LoginAsync(LoginDTO dto)
    {
        if (dto is null)
        {
            return new LoginResponse(false, null!, "Login container is empty");
        }

        var getUser = await userManager.FindByEmailAsync(dto.Email);
        if (getUser is null)
        {
            getUser = await userManager.FindByNameAsync(dto.Email);
            if (getUser is null)
            {
                return new LoginResponse(false, null!, "User not found");
            }
        }


        bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, dto.Password);
        if (!checkUserPasswords)
        {
            return new LoginResponse(false, null!, "Invalid email/password");
        }

        var getUserRole = await userManager.GetRolesAsync(getUser);
        string token = GenerateToken(getUser.Id, getUser.UserName, getUser.Email, getUserRole.First());

        return new LoginResponse(true, token!, "Login completed");
    }

    public async Task<GeneralResponse> RegisterAsync(RegisterDTO dto)
    {
        // Copy data from RegisterViewModel to IdentityUser
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var roleToCreate = Constants.USER;

        if (!await roleManager.RoleExistsAsync(roleToCreate))
        {
            var createRole = await CreateRole(roleToCreate);
            if (!createRole.Flag)
            {
                return new GeneralResponse(Flag: false, Message: createRole.Message);
            }
        }

        // Store user data in AspNetUsers database table
        var result = await userManager.CreateAsync(user, dto.Password);

        // If user is successfully created, sign-in the user using
        // SignInManager and redirect to index action of HomeController
        if (result.Succeeded)
        {
            if (!await userManager.IsInRoleAsync(user, Constants.USER))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, Constants.USER);
                if (!addRoleResult.Succeeded)
                {
                    return new GeneralResponse(Flag: false, Message: string.Join("$$$", addRoleResult.Errors.Select(e => e.Description)));
                }
            }

            if (userManager.Options.SignIn.RequireConfirmedEmail)
            {
                var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);


                /*
                What Happens If You Don't Encode the Token?

                Sending the raw token in a URL (query parameters) might corrupt it due to special characters.
                Some email clients may alter certain characters (+ might be treated as a space).
                JSON serialization might escape special characters incorrectly, making it harder to use.

                Encoding ensures the token remains valid and works across different systems without corruption. 🚀

                source: ChatGPT
                
                */

                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmToken));

                var confirmationLink = (webHostEnvironment.IsDevelopment() ? configuration.GetConnectionString("BaseUrl") : Environment.GetEnvironmentVariable("BaseUrl")) + $"/confirm-email?userId={user.Id}&token={encodedToken}";

                var smtpInfo = webHostEnvironment.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

                Helpers.SendEmail(subject: "confirm email", senderEmail: smtpInfo[0], senderPassword: smtpInfo[1], body: confirmationLink, receivers: [user.Email]);

                return new GeneralResponse(Flag: true, Message: "Registration Successful! Please confirm your email to login.");
            }
            else
            {
                await signInManager.SignInAsync(user, isPersistent: false);
            }

            return new GeneralResponse(Flag: true, Message: "Registered!");
        }

        return new GeneralResponse(Flag: false, Message: string.Join("$$$", result.Errors.Select(e => e.Description)));

    }

    public async Task<GeneralResponse> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user != null)
        {
            var result = await userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token)), dto.Password);
            if (result.Succeeded)
            {
                // Upon successful password reset and if the account is lockedout, set
                // the account lockout end date to current UTC date time, so the user
                // can login with the new password
                if (await userManager.IsLockedOutAsync(user))
                {
                    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                }
                
                return new GeneralResponse(Flag: true, Message: "Your password has been reset. Please login.");
            }

            return new GeneralResponse(Flag: false, Message: string.Join("$$$", result.Errors.Select(e => e.Description)));
        }

        return new GeneralResponse(Flag: false, Message: "couldn't find user");
    }

    private string GenerateToken(string userId, string userName, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webHostEnvironment.IsDevelopment() ? configuration["Jwt:Key"] : Environment.GetEnvironmentVariable("Jwt_Key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: webHostEnvironment.IsDevelopment() ? configuration["Jwt:Issuer"] : Environment.GetEnvironmentVariable("Jwt_Issuer"),
            audience: webHostEnvironment.IsDevelopment() ? configuration["Jwt:Audience"] : Environment.GetEnvironmentVariable("Jwt_Audience"),
            claims: userClaims,
            expires: JwtConfig.JWT_TOKEN_EXP_DATETIME,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

