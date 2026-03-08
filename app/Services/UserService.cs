using app.DTOs;
using Flurl.Http;
using Microsoft.AspNetCore.Components;

namespace app.Services;

public class UserService(SessionService sessionService, NavigationManager navigation)
{
    private readonly string _baseURL = $"{navigation.BaseUri}api/user";
    private async Task<Dictionary<string, string>> GetHttpRequestHeaders()
    {
        ProfileResponse? profile = await sessionService.GetSessionProfile();
        string? token = profile?.Token;

        Dictionary<string, string> headers = new()
        {
            { "Content-Type", "application/json" }
        };

        if (!string.IsNullOrWhiteSpace(token))
        {
            headers["Authorization"] = $"Bearer {token}";
        }

        return headers;
    }

    // GetInfo hämtar detaljerad information om en specifik användare.
    public async Task<ApiResult<UserInfoResponse>> GetInfo(PostGetRequest request)
    {
        try
        {
            UserInfoResponse? response = await $"{_baseURL}/info/{request.Page}?targetID={request.TargetID}"
                .WithHeaders(await GetHttpRequestHeaders())
                .GetJsonAsync<UserInfoResponse>();

            return new ApiResult<UserInfoResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<UserInfoResponse>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }

    // SetSettings uppdaterar användarens inställningar.
    public async Task<ApiResult<UserSettings>> SetSettings(UserSettings settings)
    {
        try
        {
            UserSettings? response = await $"{_baseURL}/settings"
                .WithHeaders(await GetHttpRequestHeaders())
                .PutJsonAsync(settings)
                .ReceiveJson<UserSettings>();

            return new ApiResult<UserSettings>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<UserSettings>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }

    // List hämtar en paginerad lista över användare.
    public async Task<ApiResult<DataPaginatedResponse<UserItemResponse>>> List(int page)
    {
        try
        {
            DataPaginatedResponse<UserItemResponse>? response = await $"{_baseURL}/list?page={page}"
                .WithHeaders(await GetHttpRequestHeaders())
                .GetJsonAsync<DataPaginatedResponse<UserItemResponse>>();

            return new ApiResult<DataPaginatedResponse<UserItemResponse>>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<DataPaginatedResponse<UserItemResponse>>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }

    // Delete tar bort en specifik användare.
    public async Task<ApiResult<SuccessfulResponse>> Delete(int targetID)
    {
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/{targetID}"
                .WithHeaders(await GetHttpRequestHeaders())
                .DeleteAsync()
                .ReceiveJson<SuccessfulResponse>();

            return new ApiResult<SuccessfulResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<SuccessfulResponse>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}