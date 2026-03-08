using app.DTOs;
using Flurl.Http;
using Microsoft.AspNetCore.Components;

namespace app.Services;

public class PostService(SessionService sessionService, NavigationManager navigation)
{
    private readonly string _baseURL = $"{navigation.BaseUri}api/posts";
    private async Task<Dictionary<string, string>> GetHttpRequestHeaders(bool hasToBeLoggedIn)
    {
        Dictionary<string, string> headers = new()
        {
            { "Content-Type", "application/json" }
        };

        if (!hasToBeLoggedIn)
        {
            return headers;
        }

        ProfileResponse? profile = await sessionService.GetSessionProfile();
        string? token = profile?.Token;

        if (!string.IsNullOrWhiteSpace(token))
        {
            headers["Authorization"] = $"Bearer {token}";
        }

        return headers;
    }

    // Get hämtar en paginerad lista över inlägg för en specifik anime.
    public async Task<ApiResult<DataPaginatedResponse<PostResponse>>> Get(PostGetRequest request)
    {
        try
        {
            DataPaginatedResponse<PostResponse>? response = await $"{_baseURL}/{request.TargetID}/{request.Page}?timezone={request.TimeZone}"
                .WithHeaders(await GetHttpRequestHeaders(false))
                .GetJsonAsync<DataPaginatedResponse<PostResponse>>();

            return new ApiResult<DataPaginatedResponse<PostResponse>>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<DataPaginatedResponse<PostResponse>>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }

    // Post skapar ett nytt inlägg.
    public async Task<ApiResult<SuccessfulResponse>> Post(PostRequest request)
    {
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/send"
                .WithHeaders(await GetHttpRequestHeaders(true))
                .PostJsonAsync(request)
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

    // Put uppdaterar ett befintligt inlägg.
    public async Task<ApiResult<SuccessfulResponse>> Put(PostRequest request)
    {
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/edit"
                .WithHeaders(await GetHttpRequestHeaders(true))
                .PutJsonAsync(request)
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

    // Delete tar bort ett specifikt inlägg.
    public async Task<ApiResult<SuccessfulResponse>> Delete(int targetID)
    {
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/delete/{targetID}"
                .WithHeaders(await GetHttpRequestHeaders(true))
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