using app.DTOs;
using Flurl.Http;
using Microsoft.AspNetCore.Components;

namespace app.Services;

public class PostService(SessionService sessionService, NavigationManager navigation)
{
    private readonly string _baseURL = $"{navigation.BaseUri}api/posts";
    private string? _token { get; set; } = null;
    private bool _initialized = false;

    public async Task Initialize()
    {
        if (_initialized) return;

        ProfileResponse? profile = await sessionService.GetSessionProfile();
        _token = profile?.Token;
        _initialized = true;
    }

    private async Task EnsureInitialized()
    {
        if (!_initialized)
        {
            await Initialize();
        }
    }

    private Dictionary<string, string> GetHttpRequestHeaders(bool hasToBeLoggedIn) =>
    hasToBeLoggedIn
        ? !string.IsNullOrWhiteSpace(_token)
            ? new()
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {_token}" }
            }
            : new()
            {
                { "Content-Type", "application/json" }
            }
        : new()
        {
            { "Content-Type", "application/json" }
        };

    // Get hämtar en paginerad lista över inlägg för en specifik anime.
    public async Task<ApiResult<DataPaginatedResponse<PostResponse>>> Get(PostGetRequest request)
    {
        try
        {
            DataPaginatedResponse<PostResponse>? response = await $"{_baseURL}/{request.TargetID}/{request.Page}?timezone={request.TimeZone}"
                .WithHeaders(GetHttpRequestHeaders(false))
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
        await EnsureInitialized();
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/send"
                .WithHeaders(GetHttpRequestHeaders(true))
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
        await EnsureInitialized();
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/edit"
                .WithHeaders(GetHttpRequestHeaders(true))
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
        await EnsureInitialized();
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/delete/{targetID}"
                .WithHeaders(GetHttpRequestHeaders(true))
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