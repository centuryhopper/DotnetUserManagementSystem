using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models;


namespace Server.Controllers;

[Authorize(Roles = Shared.Constants.ADMIN)]
[Route("api/[controller]")]
[ApiController]
public class ApplicationsController(IApplicationsRepository applicationsRepository, ILogger<ApplicationsController> logger) : ControllerBase
{
    [HttpGet("get-applications")]
    public async Task<IActionResult> GetApplicationsAsync()
    {
        try
        {
            var response = await applicationsRepository.GetApplicationsAsync();
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpPost("add-application")]
    public async Task<IActionResult> AddRoles([FromBody] ApplicationDTO dto)
    {
        try
        {
            var response = await applicationsRepository.AddApplicationAsync(dto);
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpPost("add-applications")]
    public async Task<IActionResult> AddApplicationsAsync([FromBody] IEnumerable<ApplicationDTO> dtos)
    {
        try
        {
            var response = await applicationsRepository.AddApplicationsAsync(dtos);
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpPatch("edit-application")]
    public async Task<IActionResult> EditRole([FromBody] ApplicationDTO dto)
    {
        try
        {
            var response = await applicationsRepository.EditApplicationAsync(dto);
            if (!response.Flag)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }

    [HttpDelete("delete-application/{applicationId:int}")]
    public async Task<IActionResult> DeleteRole(int applicationId)
    {
        try
        {
            var response = await applicationsRepository.DeleteApplicationAsync(applicationId);
            if (!response.Flag)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }
    }


}
