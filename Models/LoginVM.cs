using System.ComponentModel.DataAnnotations;
namespace DotnetUserManagementSystem.Models;

public class LoginVM
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
