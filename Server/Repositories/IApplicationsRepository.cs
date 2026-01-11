
using Microsoft.AspNetCore.Identity;
using Server.Entities;
using Shared.Models;



public interface IApplicationsRepository
{
    Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync();
    Task<IEnumerable<ApplicationDTO>> GetApplicationsByAppNameAndUserIdAsync(string appName, string userId);
    Task<GeneralResponseWithPayload<string>> AddApplicationAsync(ApplicationDTO dto);
    Task<IEnumerable<GeneralResponseWithPayload<string>>> AddApplicationsAsync(IEnumerable<ApplicationDTO> dtos);
    Task<GeneralResponseWithPayload<string>> EditApplicationAsync(ApplicationDTO dto);
    Task<GeneralResponse> DeleteApplicationAsync(int applicationId);
}