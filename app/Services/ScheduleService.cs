using app.DTOs;
using Flurl.Http;
using Microsoft.AspNetCore.Components;

namespace app.Services;

public class ScheduleService(SessionService sessionService, NavigationManager navigation)
{
    private readonly string _baseURL = $"{navigation.BaseUri}api/schedule";
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

    private Dictionary<string, string> GetHttpRequestHeaders()
    {
        Dictionary<string, string> headers = new()
        {
            { "Content-Type", "application/json" }
        };

        if (!string.IsNullOrWhiteSpace(_token))
        {
            headers["Authorization"] = $"Bearer {_token}";
        }

        return headers;
    }

    // Get hämtar hela schemat för den inloggade användaren.
    public async Task<ApiResult<ScheduleResponse>> Get()
    {
        await EnsureInitialized();
        try
        {
            ScheduleResponse? response = await $"{_baseURL}/schedule"
                .WithHeaders(GetHttpRequestHeaders())
                .GetJsonAsync<ScheduleResponse>();

            return new ApiResult<ScheduleResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (FlurlHttpException ex)
        {
            ApiError? error = await ResponseHandling.HandleError(ex);
            return new ApiResult<ScheduleResponse>
            {
                IsSuccess = false,
                Error = error
            };
        }
    }

    // Post skapar en ny schemapost i användarens schema.
    public async Task<ApiResult<SuccessfulResponse>> Post(ScheduleRequest request)
    {
        await EnsureInitialized();
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/entry"
                .WithHeaders(GetHttpRequestHeaders())
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

    // Put uppdaterar en befintlig schemapost i användarens schema.
    public async Task<ApiResult<SuccessfulResponse>> Put(ScheduleUpdateRequest request)
    {
        await EnsureInitialized();
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/entry"
                .WithHeaders(GetHttpRequestHeaders())
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

    // Delete tar bort en schemapost från användarens schema baserat på postens ID.
    public async Task<ApiResult<SuccessfulResponse>> Delete(int entryId)
    {
        await EnsureInitialized();
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/entry/{entryId}"
                .WithHeaders(GetHttpRequestHeaders())
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