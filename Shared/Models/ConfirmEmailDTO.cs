
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class ConfirmEmailDTO
{
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
}