
using Microsoft.AspNetCore.Identity;

namespace DotnetUserManagementSystem.Contexts;

public class ApplicationRole : IdentityRole
{
    public bool IsActive { get; set; }
}