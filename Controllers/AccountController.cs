using DotnetUserManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DotnetUserManagementSystem.Contexts;
using Portfolio.Utilities;

namespace DotnetUserManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IHostEnvironment env;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IHostEnvironment env)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.env = env;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginVM vm, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    vm.Email, vm.Password, vm.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

                // ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                TempData[TempDataKeys.ALERT_ERROR] = "Invalid login attempt!";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return View(vm);
        }

        public ActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                TempData[TempDataKeys.ALERT_ERROR] = "Missing user id or token.";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                TempData[TempDataKeys.ALERT_ERROR] = "The user id is invalid";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData[TempDataKeys.ALERT_SUCCESS] = "The email has been successfully confirmed!";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            TempData[TempDataKeys.ALERT_ERROR] = "We couldn't confirm your email.";
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        private async Task<IdentityResult> CreateRole(string roleName)
        {
            // We just need to specify a unique role name to create a new role
            ApplicationRole role = new ApplicationRole
            {
                Name = roleName
            };

            // Saves the role in the underlying AspNetRoles table
            IdentityResult result = await roleManager.CreateAsync(role);

            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new ApplicationUser
                {
                    UserName = vm.Email,
                    Email = vm.Email
                };

                var roleToCreate = "Admin";

                if (!await roleManager.RoleExistsAsync(roleToCreate))
                {
                    var createRole = await CreateRole(roleToCreate);
                    if (!createRole.Succeeded)
                    {
                        foreach (var error in createRole.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View(vm);
                    }
                }


                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, vm.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    if (!await userManager.IsInRoleAsync(user, "Admin"))
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                        if (!addRoleResult.Succeeded)
                        {
                            foreach (var error in addRoleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                            return View(vm);
                        }
                    }

                    var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = emailConfirmToken }, Request.Scheme);

                    var smtpInfo = env.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

                    Helpers.SendEmail(subject: "confirm email", senderEmail: smtpInfo[0], senderPassword: smtpInfo[1], body: confirmationLink, receivers: [user.Email]);

                    // await signInManager.SignInAsync(user, isPersistent: false);

                    TempData[TempDataKeys.ALERT_SUCCESS] = "Registration Successful! Please confirm your email to login.";

                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }




            return View(vm);
        }

        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}
