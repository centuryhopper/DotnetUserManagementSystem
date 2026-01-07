
namespace Shared.Models;


public class App2FASettingsDTO
{
    public string Email { get; set; } = null!;
    public string AppName { get; set; } = null!;
    public bool Enable2FA { get; set; }
}