
using System.ComponentModel.DataAnnotations;

namespace DotnetUserManagementSystem.Models;

public class ResetPasswordLinkVM
{
    [Display(Name = "Email"), Required(ErrorMessage = "Please enter your email")]
    public string Email { get; set; }
}