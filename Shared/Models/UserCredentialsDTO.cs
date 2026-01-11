

namespace Shared.Models;

public class UserCredentialsDTO
{
    public string Username;
    public string Email;
    public IEnumerable<string> Roles;
    public string UserId;
}