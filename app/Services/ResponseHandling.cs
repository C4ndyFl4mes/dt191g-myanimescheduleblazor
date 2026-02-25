using app.DTOs;
using Flurl.Http;

namespace app.Services;

public class ResponseHandling
{
    // HandleError hanterar fel som uppstår under HTTP-anrop och returnerar en strukturerad ApiError.
    public static async Task<ApiError> HandleError(FlurlHttpException ex)
    {
        if (ex.Call.Response != null)
        {
            var error = await ex.GetResponseJsonAsync<RawApiError>();

            if (error == null)
            {
                return new ApiError
                {
                    StatusCode = ex.Call.Response.StatusCode,
                    Title = "An unknown error occurred.",
                    Details = new[] { "Unable to parse error response." }
                };
            }


            Console.WriteLine($"Debugging: Received error {error.Detail} with type {error.Type}");

            string[] detailsArray = [];

            if (error.Type == "ValidationException")
            {
                detailsArray = error.Detail?.Split("\r\n -- ").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray() ?? Array.Empty<string>();
            }
            else
            {
                detailsArray = new[] { error.Detail ?? "An unknown error occurred." };
            }

            ApiError apiError = new()
            {
                StatusCode = ex.Call.Response.StatusCode,
                Title = error.Title,
                Details = detailsArray
            };

            return apiError;
        }
        else
        {
            return new ApiError
            {
                StatusCode = 0,
                Title = "An unknown error occurred.",
                Details = new[] { "An unknown error occurred." }
            };
        }
    }
}