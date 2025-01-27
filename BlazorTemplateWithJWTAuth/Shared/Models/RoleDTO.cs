using System.ComponentModel.DataAnnotations;
namespace Shared.Models;

public class RoleDTO
{
    public string? Id { get; set; }
    [Display(Name = "Name")]
    public string RoleName { get; set; }
    [Display(Name = "Is Active?")]
    public bool IsActive { get; set; }
}
