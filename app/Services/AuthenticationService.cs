using app.DTOs;
using Flurl.Http;
using Microsoft.AspNetCore.Components;

namespace app.Services;

public class AuthenticationService(NavigationManager navigation)
{
    private readonly string _baseURL = $"{navigation.BaseUri}api/user";
    private readonly Dictionary<string, string> _httpRequestHeaders = new()
    {
        { "Content-Type", "application/json" }
    };

    // SignUp skapar ett nytt konto för användaren och returnerar profilinformationen inklusive token.
    public async Task<ApiResult<ProfileResponse>> SignUp(SignUpRequest request)
    {
        try
        {
            IFlurlResponse response = await $"{_baseURL}/signup"
                .WithHeaders(_httpRequestHeaders)
                .PostJsonAsync(request);

            ProfileResponse profile = await response.GetJsonAsync<ProfileResponse>();
            return new ApiResult<ProfileResponse>
            {
                IsSuccess = true,
                Data = profile
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<ProfileResponse>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }

    // SignIn loggar in användaren och returnerar profilinformationen inklusive token.
    public async Task<ApiResult<ProfileResponse>> SignIn(SignInRequest request)
    {
        try
        {
            IFlurlResponse response = await $"{_baseURL}/signin"
                .WithHeaders(_httpRequestHeaders)
                .PostJsonAsync(request);
            
            ProfileResponse profile = await response.GetJsonAsync<ProfileResponse>();
            return new ApiResult<ProfileResponse>
            {
                IsSuccess = true,
                Data = profile
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<ProfileResponse>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}