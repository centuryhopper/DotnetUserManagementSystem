using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class TwoFactorDTO
{
    public string UserId { get; set; }
    public string Code { get; set; }
}

