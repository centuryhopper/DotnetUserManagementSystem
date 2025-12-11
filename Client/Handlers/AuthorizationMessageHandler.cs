

using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Shared.Models;

namespace Client.Handlers;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService localStorage;
    private readonly ISessionStorageService sessionStorageService;

    public AuthorizationMessageHandler(ILocalStorageService localStorage, ISessionStorageService sessionStorageService)
    {
        this.sessionStorageService = sessionStorageService;
        this.localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await sessionStorageService.GetItemAsync<string>(JwtConfig.JWT_TOKEN_NAME) ?? await localStorage.GetItemAsync<string>(JwtConfig.JWT_TOKEN_NAME);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
