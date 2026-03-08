using app.DTOs;
using Flurl.Http;
using Microsoft.AspNetCore.Components;

namespace app.Services;

public class ScheduleService(SessionService sessionService, NavigationManager navigation)
{
    private readonly string _baseURL = $"{navigation.BaseUri}api/schedule";
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

    // Get hämtar hela schemat för den inloggade användaren.
    public async Task<ApiResult<ScheduleResponse>> Get()
    {
        try
        {
            ScheduleResponse? response = await $"{_baseURL}/schedule"
                .WithHeaders(await GetHttpRequestHeaders())
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
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/entry"
                .WithHeaders(await GetHttpRequestHeaders())
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
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/entry"
                .WithHeaders(await GetHttpRequestHeaders())
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
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/entry/{entryId}"
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