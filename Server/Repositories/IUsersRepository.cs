
using Microsoft.AspNetCore.Identity;
using Server.Entities;
using Shared.Models;
using static Shared.Models.ServiceResponses;


public interface IUsersRepository
{
    Task<IEnumerable<UserDTO>> GetUsersAsync();
}