
using Microsoft.AspNetCore.Identity;
using Server.Entities;
using Shared.Models;



public interface IApplicationsRepository
{
    Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync();
    Task<IEnumerable<ApplicationDTO>> GetApplicationsByAppNameAndUserIdAsync(string appName, string userId);
    Task<HandyGeneralResponseWithPayload> AddApplicationAsync(ApplicationDTO dto);
    Task<IEnumerable<HandyGeneralResponseWithPayload>> AddApplicationsAsync(IEnumerable<ApplicationDTO> dtos);
    Task<HandyGeneralResponseWithPayload> EditApplicationAsync(ApplicationDTO dto);
    Task<HandyGeneralResponse> DeleteApplicationAsync(int applicationId);
}