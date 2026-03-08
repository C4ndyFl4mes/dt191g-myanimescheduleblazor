using app.DTOs;
using Flurl.Http;
using System.Text.Json;

namespace app.Services;

public class ResponseHandling
{
    // HandleError hanterar fel som uppstår under HTTP-anrop och returnerar en strukturerad ApiError.
    public static async Task<ApiError> HandleError(FlurlHttpException ex)
    {
        int statusCode = ex.Call?.Response?.StatusCode ?? 0;

        if (ex.Call?.Response is null)
        {
            return new ApiError
            {
                StatusCode = statusCode,
                Title = "An unknown error occurred.",
                Details = new[] { "An unknown error occurred." }
            };
        }

        RawApiError? error = null;

        try
        {
            error = await ex.GetResponseJsonAsync<RawApiError>();
        }
        catch (Exception)
        {
            // Ignorera eventuella fel som uppstår under deserialisering av felmeddelandet.
        }

        if (error is null)
        {
            return new ApiError
            {
                StatusCode = statusCode,
                Title = "An unknown error occurred.",
                Details = new[] { "Unable to parse error response." }
            };
        }

        string[] detailsArray;
        if (error.Type == "ValidationException")
        {
            detailsArray = error.Detail?
                .Split("\r\n -- ")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray() ?? Array.Empty<string>();

            if (detailsArray.Length == 0)
            {
                detailsArray = new[] { "Validation failed." };
            }
        }
        else
        {
            detailsArray = new[] { error.Detail ?? "An unknown error occurred." };
        }

        return new ApiError
        {
            StatusCode = statusCode,
            Title = string.IsNullOrWhiteSpace(error.Title) ? "Request failed." : error.Title,
            Details = detailsArray
        };
    }
}