
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

public class UsersGridStateService : HandyGridStateAbstract<UsersGridEntity, UserDTO>
{
    public UsersGridStateService(List<UsersGridEntity> Items, int PageSize = 5, bool CanAddNewItems = true, string? ExampleFileUploadUrl = null, string AddNewItemsText = "Add New Items", bool Exportable = false, bool IsReadonly = true, bool ShowRowIndex = true, bool ShowFilters = true, Func<IEnumerable<UsersGridEntity>, Task>? OnCreate = null, Func<UsersGridEntity, Task>? OnUpdate = null, Func<UsersGridEntity, Task>? OnDelete = null, Func<IEnumerable<UsersGridEntity>, Task>? OnSubmitFile = null, List<string>? ColumnsToHide = null, List<string>? ReadonlyColumns = null, List<HandyNamedRenderFragment<UsersGridEntity>>? ViewModeFragments = null, List<HandyNamedRenderFragment<UsersGridEntity>>? EditModeFragments = null) : base(Items, PageSize, CanAddNewItems, ExampleFileUploadUrl, AddNewItemsText, Exportable, IsReadonly, ShowRowIndex, ShowFilters, OnCreate, OnUpdate, OnDelete, OnSubmitFile, ColumnsToHide, ReadonlyColumns, ViewModeFragments, EditModeFragments)
    {
    }

    public override HandyGridValidationResponse ValidationChecks(UsersGridEntity item)
    {
        base.ValidationChecks(item);

        if (ErrorMessagesDict.Any())
        {
            return new HandyGridValidationResponse(Flag: false, ErrorMessagesDict);
        }

        return new HandyGridValidationResponse(Flag: true, null);
    }
}



