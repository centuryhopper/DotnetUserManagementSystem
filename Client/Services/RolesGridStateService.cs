
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

public class RolesGridStateService : HandyGridStateAbstract<RolesGridEntity, RoleDTO>
{
    public RolesGridStateService(List<RolesGridEntity> Items, int PageSize = 5, bool CanAddNewItems = true, string? ExampleFileUploadUrl = null, string AddNewItemsText = "Add New Items", bool Exportable = false, bool IsReadonly = true, bool ShowRowIndex = true, bool ShowFilters = true, Func<IEnumerable<RolesGridEntity>, Task>? OnCreate = null, Func<RolesGridEntity, Task>? OnUpdate = null, Func<RolesGridEntity, Task>? OnDelete = null, Func<IEnumerable<RolesGridEntity>, Task>? OnSubmitFile = null, List<string>? ColumnsToHide = null, List<string>? ReadonlyColumns = null, List<HandyNamedRenderFragment<RolesGridEntity>>? ViewModeFragments = null, List<HandyNamedRenderFragment<RolesGridEntity>>? EditModeFragments = null) : base(Items, PageSize, CanAddNewItems, ExampleFileUploadUrl, AddNewItemsText, Exportable, IsReadonly, ShowRowIndex, ShowFilters, OnCreate, OnUpdate, OnDelete, OnSubmitFile, ColumnsToHide, ReadonlyColumns, ViewModeFragments, EditModeFragments)
    {
    }

    public override HandyGridValidationResponse ValidationChecks(RolesGridEntity item)
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
            return new HandyGridValidationResponse(Flag: false, ErrorMessagesDict);
        }

        return new HandyGridValidationResponse(Flag: true, null);
    }
}



