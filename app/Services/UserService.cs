using app.DTOs;
using Flurl.Http;

namespace app.Services;

public class UserService(SessionService sessionService)
{
    private readonly string _baseURL = "http://localhost:5083/api/user";
    private string? _token = null;
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

    // GetInfo hämtar detaljerad information om en specifik användare.
    public async Task<ApiResult<UserInfoResponse>> GetInfo(PostGetRequest request)
    {
        await EnsureInitialized();
        try
        {
            UserInfoResponse? response = await $"{_baseURL}/info/{request.Page}?targetID={request.TargetID}"
                .WithHeaders(GetHttpRequestHeaders())
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
        await EnsureInitialized();
        try
        {
            UserSettings? response = await $"{_baseURL}/settings"
                .WithHeaders(GetHttpRequestHeaders())
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
        await EnsureInitialized();
        try
        {
            DataPaginatedResponse<UserItemResponse>? response = await $"{_baseURL}/list?page={page}"
                .WithHeaders(GetHttpRequestHeaders())
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
        await EnsureInitialized();
        try
        {
            SuccessfulResponse? response = await $"{_baseURL}/{targetID}"
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