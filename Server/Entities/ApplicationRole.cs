
using Microsoft.AspNetCore.Identity;

namespace Server.Entities;

public class ApplicationRole : IdentityRole
{
    public bool IsActive { get; set; }
}