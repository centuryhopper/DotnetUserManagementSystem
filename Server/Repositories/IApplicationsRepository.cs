
using Microsoft.AspNetCore.Identity;
using Server.Entities;
using Shared.Models;



public interface IApplicationsRepository
{
    Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync();
    Task<IEnumerable<ApplicationDTO>> GetApplicationsByAppNameAndUserIdAsync(string appName, string userId);
    Task<GeneralResponseWithPayload> AddApplicationAsync(ApplicationDTO dto);
    Task<IEnumerable<GeneralResponseWithPayload>> AddApplicationsAsync(IEnumerable<ApplicationDTO> dtos);
    Task<GeneralResponseWithPayload> EditApplicationAsync(ApplicationDTO dto);
    Task<GeneralResponse> DeleteApplicationAsync(int applicationId);
}