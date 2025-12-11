

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Client.Interfaces;
using Client.Providers;
using Client.Utils;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models;


namespace Client.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorageService;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly ISessionStorageService sessionStorageService;

    public AccountService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider, ISessionStorageService sessionStorageService)
    {
        httpClient = httpClientFactory.CreateClient(Constants.HTTP_CLIENT);
        this.localStorageService = localStorageService;
        this.authenticationStateProvider = authenticationStateProvider;
        this.sessionStorageService = sessionStorageService;
    }

    public async Task<HandyLoginResponse> LoginAsync(LoginDTO dto)
    {
        try
        {
            //System.Console.WriteLine("logging in");
            var response = await httpClient.PostAsJsonAsync("api/Account/login", dto);
            var loginResponse = await response.Content.ReadFromJsonAsync<HandyLoginResponse>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(loginResponse!.Message);
            }
            if (string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                throw new Exception(loginResponse.Message);
            }

            if (dto.RememberMe)
            {
                await localStorageService.SetItemAsync(JwtConfig.JWT_TOKEN_NAME, loginResponse!.Token);
            }
            else
            {
                await sessionStorageService.SetItemAsync(JwtConfig.JWT_TOKEN_NAME, loginResponse!.Token);
            }

            ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResponse!.Token);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse!.Token);

            return loginResponse!;
        }
        catch (Exception ex)
        {
            return new HandyLoginResponse(Flag: false, Token: "", Message: ex.Message);
        }
    }

    public async Task LogoutAsync()
    {
        await localStorageService.RemoveItemAsync(JwtConfig.JWT_TOKEN_NAME);
        await sessionStorageService.RemoveItemAsync(JwtConfig.JWT_TOKEN_NAME);
        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<HandyGeneralResponse> RegisterAsync(RegisterDTO dto)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/Account/register", dto);
            var registerResponse = await response.Content.ReadFromJsonAsync<HandyGeneralResponse>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(registerResponse!.Message);
            }

            if (!registerResponse.Flag)
            {
                return new HandyGeneralResponse(false, registerResponse!.Message);
            }

            return new HandyGeneralResponse(true, registerResponse!.Message);
        }
        catch (Exception ex)
        {
            return new HandyGeneralResponse(false, ex.Message);
        }
    }

    public async Task<HandyGeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO dto)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/Account/forgot-password", dto);
            var forgotPasswordResponse = await response.Content.ReadFromJsonAsync<HandyGeneralResponse>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(forgotPasswordResponse!.Message);
            }

            if (!forgotPasswordResponse.Flag)
            {
                return new HandyGeneralResponse(false, forgotPasswordResponse!.Message);
            }

            return new HandyGeneralResponse(true, forgotPasswordResponse!.Message);
        }
        catch (Exception ex)
        {
            return new HandyGeneralResponse(false, ex.Message);
        }
    }

    public async Task<HandyGeneralResponse> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/Account/reset-password", dto);
            var resetPasswordResponse = await response.Content.ReadFromJsonAsync<HandyGeneralResponse>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(resetPasswordResponse!.Message);
            }

            if (!resetPasswordResponse.Flag)
            {
                return new HandyGeneralResponse(false, resetPasswordResponse!.Message);
            }

            return new HandyGeneralResponse(true, resetPasswordResponse!.Message);
        }
        catch (Exception ex)
        {
            return new HandyGeneralResponse(false, ex.Message);
        }
    }

    public async Task<HandyGeneralResponse> EditProfileAsync(ProfileDTO dto)
    {
        try
        {
            var response = await httpClient.PatchAsJsonAsync("api/Account/edit-profile", dto);
            var editProfileResponse = await response.Content.ReadFromJsonAsync<HandyGeneralResponse>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(editProfileResponse!.Message);
            }

            if (!editProfileResponse.Flag)
            {
                return new HandyGeneralResponse(false, editProfileResponse!.Message);
            }

            return new HandyGeneralResponse(true, editProfileResponse!.Message);
        }
        catch (Exception ex)
        {
            return new HandyGeneralResponse(false, ex.Message);
        }
    }
}
