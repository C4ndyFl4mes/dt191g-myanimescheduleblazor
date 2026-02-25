using app.DTOs;
using Flurl.Http;

namespace app.Services;

public class ScheduleService(SessionService sessionService)
{
    private readonly string _baseURL = "http://localhost:5083/api/schedule";
    private string? _token { get; set; } = null;
    
    public async Task Initialize()
    {
        ProfileResponse? profile = await sessionService.GetSessionProfile();
        if (profile != null)
        {
            _token = profile.Token;
        }
    }

    private Dictionary<string, string> GetHttpRequestHeaders() => new()
    {
        { "Content-Type", "application/json" },
        { "Authorization", $"Bearer {_token}" }
    };

    // Get hämtar hela schemat för den inloggade användaren.
    public async Task<ApiResult<ScheduleResponse>> Get()
    {
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