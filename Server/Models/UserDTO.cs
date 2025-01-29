using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class UserDTO
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; }
}

