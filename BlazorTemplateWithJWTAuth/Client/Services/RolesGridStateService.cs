
namespace Client.Services;

using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Client.Models;
using Client.Utils;
using HandyBlazorComponents.Abstracts;
using HandyBlazorComponents.Models;
using Microsoft.AspNetCore.Components;
using Shared.Models;
using static HandyBlazorComponents.Models.ServiceResponses;

public class RolesGridStateService : HandyGridStateAbstract<RolesGridEntity, RoleDTO>
{
    public RolesGridStateService(List<RolesGridEntity> Items, int PageSize = 5, string? ExampleFileUploadUrl = null, bool Exportable = false, bool IsReadonly = true, Func<IEnumerable<RolesGridEntity>, Task>? OnCreate = null, Func<RolesGridEntity, Task>? OnUpdate = null, Func<RolesGridEntity, Task>? OnDelete = null, Func<IEnumerable<RolesGridEntity>, Task>? OnSubmitFile = null, List<string>? ColumnsToHide = null, List<string>? ReadonlyColumns = null, List<NamedRenderFragment<RolesGridEntity>>? ViewModeFragments = null, List<NamedRenderFragment<RolesGridEntity>>? EditModeFragments = null) : base(Items, PageSize, ExampleFileUploadUrl, Exportable, IsReadonly, OnCreate, OnUpdate, OnDelete, OnSubmitFile, ColumnsToHide, ReadonlyColumns, ViewModeFragments, EditModeFragments)
    {
    }

    public override GridValidationResponse ValidationChecks(RolesGridEntity item)
    {
        base.ValidationChecks(item);
        
        if (string.IsNullOrWhiteSpace(item.Object.RoleName))
        {
            ErrorMessagesDict[nameof(item.Object.RoleName)].Add($"Please fill out {nameof(item.Object.RoleName)}");
        }
        if (!string.IsNullOrWhiteSpace(item.Object.RoleName) && item.Object.RoleName?.Length > 256)
        {
            ErrorMessagesDict[nameof(item.Object.RoleName)].Add("Please make sure all fields are under 256 characters");
        }

        if (ErrorMessagesDict.Any())
        {
            return new GridValidationResponse(Flag: false, ErrorMessagesDict);
        }

        return new GridValidationResponse(Flag: true, null);
    }
}



