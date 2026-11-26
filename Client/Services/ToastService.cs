using Microsoft.JSInterop;

namespace Client.Services;

public class ToastService
{
    private readonly IJSRuntime jsRuntime;

    public ToastService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task Error(string toastTitle, string msg)
    {
        await jsRuntime.InvokeVoidAsync("NOTIFY.toast", "error", msg, toastTitle);
    }

    public async Task Success(string toastTitle, string msg)
    {
        await jsRuntime.InvokeVoidAsync("NOTIFY.toast", "success", msg, toastTitle);
    }

    public async Task Warning(string toastTitle, string msg)
    {
        await jsRuntime.InvokeVoidAsync("NOTIFY.toast", "warning", msg, toastTitle);
    }

    public async Task Info(string toastTitle, string msg)
    {
        await jsRuntime.InvokeVoidAsync("NOTIFY.toast", "info", msg, toastTitle);
    }
}