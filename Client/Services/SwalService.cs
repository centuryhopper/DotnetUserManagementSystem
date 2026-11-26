using Microsoft.JSInterop;

namespace Client.Services;

public class SwalService
{
    private readonly IJSRuntime jsRuntime;

    public SwalService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task<bool> Confirm(string title, string msg)
    {
        return await jsRuntime.InvokeAsync<bool>("NOTIFY.confirm", title, msg);
    }

    public async Task Alert(string icon, string title, string msg)
    {
        await jsRuntime.InvokeVoidAsync("NOTIFY.alert", icon, title, msg);
    }
}