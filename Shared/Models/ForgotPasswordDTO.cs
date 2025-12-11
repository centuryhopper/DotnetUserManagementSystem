
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class ForgotPasswordDTO
{
    [Display(Name = "Email"), Required(ErrorMessage = "Please enter your email")]
    public string Email { get; set; }
}