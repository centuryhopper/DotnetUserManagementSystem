using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Contexts;
using Server.Entities;

using Server.Utils;

namespace Server.Repositories;

public class ApplicationsRepository(UserManagementAdditionalContext ctx, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : IApplicationsRepository
{
    public async Task<GeneralResponseWithPayload> AddApplicationAsync(ApplicationDTO dto)
    {
        try
        {
            // we use app later when getting for when the primary key gets generated and returned as payload to help keep the grid Ids in sync
            var app = dto.ToEntity();
            await ctx.Applications.AddAsync(app);
            await ctx.SaveChangesAsync();
            return new GeneralResponseWithPayload(true, "Application Added", app.Applicationid.ToString());
        }
        catch (System.Exception ex)
        {
            return new GeneralResponseWithPayload(false, ex.Message, "");
        }
    }

    public async Task<IEnumerable<GeneralResponseWithPayload>> AddApplicationsAsync(IEnumerable<ApplicationDTO> dtos)
    {
        List<GeneralResponseWithPayload> responses = [];
        foreach (var dto in dtos)
        {
            responses.Add(await AddApplicationAsync(dto));
        }
        return responses;
    }

    public async Task<GeneralResponse> DeleteApplicationAsync(int applicationId)
    {
        try
        {
            var app = await ctx.Applications.FindAsync(applicationId);
            if (app == null)
            {
                return new GeneralResponse(false, "application not found");
            }
            ctx.Applications.Remove(app);
            await ctx.SaveChangesAsync();
            return new GeneralResponse(true, "Application deleted");
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(false, ex.Message);
        }
    }

    public async Task<GeneralResponseWithPayload> EditApplicationAsync(ApplicationDTO dto)
    {
        try
        {
            var app = await ctx.Applications.FindAsync(dto.Applicationid);
            if (app == null)
            {
                return new GeneralResponseWithPayload(false, "application not found", "");
            }
            app.Applicationname = dto.Applicationname;
            app.Roleid = dto.Roleid;
            app.Userid = dto.Userid;
            await ctx.SaveChangesAsync();
            return new GeneralResponseWithPayload(true, "Application updated", dto.Applicationid.ToString());
        }
        catch (System.Exception ex)
        {
            return new GeneralResponseWithPayload(false, ex.Message, "");
        }
    }

    public async Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync()
    {
        return await ctx.Applications
            .Select(app => app.ToDTO())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationDTO>> GetApplicationsByAppNameAndUserIdAsync(string appName, string userId)
    {
        return await ctx.Applications
        .Where(app => app.Applicationname.ToLower() == appName.ToLower() && userId == app.Userid).Select(app => app.ToDTO()).AsNoTracking().ToListAsync();
    }
}

