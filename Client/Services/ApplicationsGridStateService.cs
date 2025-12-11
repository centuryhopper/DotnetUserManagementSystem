
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

public class ApplicationsGridStateService : HandyGridStateAbstract<ApplicationsGridEntity, ApplicationDTO>
{
    public ApplicationsGridStateService(List<ApplicationsGridEntity> Items, int PageSize = 5, bool CanAddNewItems = true, string? ExampleFileUploadUrl = null, string AddNewItemsText = "Add New Items", bool Exportable = false, bool IsReadonly = true, bool ShowRowIndex = true, bool ShowFilters = true, Func<IEnumerable<ApplicationsGridEntity>, Task>? OnCreate = null, Func<ApplicationsGridEntity, Task>? OnUpdate = null, Func<ApplicationsGridEntity, Task>? OnDelete = null, Func<IEnumerable<ApplicationsGridEntity>, Task>? OnSubmitFile = null, List<string>? ColumnsToHide = null, List<string>? ReadonlyColumns = null, List<HandyNamedRenderFragment<ApplicationsGridEntity>>? ViewModeFragments = null, List<HandyNamedRenderFragment<ApplicationsGridEntity>>? EditModeFragments = null) : base(Items, PageSize, CanAddNewItems, ExampleFileUploadUrl, AddNewItemsText, Exportable, IsReadonly, ShowRowIndex, ShowFilters, OnCreate, OnUpdate, OnDelete, OnSubmitFile, ColumnsToHide, ReadonlyColumns, ViewModeFragments, EditModeFragments)
    {
    }

    public override HandyGridValidationResponse ValidationChecks(ApplicationsGridEntity item)
    {
        base.ValidationChecks(item);

        if (string.IsNullOrWhiteSpace(item.Object.Applicationname))
        {
            ErrorMessagesDict[nameof(item.Object.Applicationname)].Add($"Please fill out {nameof(item.Object.Applicationname)}");
        }

        if (string.IsNullOrWhiteSpace(item.Object.Userid))
        {
            ErrorMessagesDict[nameof(item.Object.Userid)].Add($"Please fill out {nameof(item.Object.Userid)}");
        }

        if (string.IsNullOrWhiteSpace(item.Object.Roleid))
        {
            ErrorMessagesDict[nameof(item.Object.Roleid)].Add($"Please fill out {nameof(item.Object.Roleid)}");
        }

        if (ErrorMessagesDict.Any())
        {
            return new HandyGridValidationResponse(Flag: false, ErrorMessagesDict);
        }

        return new HandyGridValidationResponse(Flag: true, null);
    }
}



