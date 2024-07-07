using DotnetUserManagementSystem.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotnetUserManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        //Server=ep-shy-boat-a5z9pcbn.us-east-2.aws.neon.tech;Database=UserManagement;User Id=neondb_owner;Password=NSFWkL9Zwb6f;Port=5432

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        
        public async Task<ActionResult> Index()
        {
            var users = userManager.Users.AsEnumerable().ToList();
            Dictionary<string,IEnumerable<string>> userRoles = new();
            foreach (var user in users)
            {
                userRoles.Add(user.Id, await userManager.GetRolesAsync(user));
            }
            ViewBag.UserRoles = userRoles;
            ViewBag.Users = users;

            return View();
        }

        public async Task<ActionResult> AddUser()
        {
            return View();
        }

        public async Task<ActionResult> RemoveUser()
        {
            return View();
        }

    }
}
