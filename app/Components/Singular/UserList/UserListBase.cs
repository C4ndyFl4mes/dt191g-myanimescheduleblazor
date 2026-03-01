using app.DTOs;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class UserListBase : ComponentBase
{
    [Inject]
    protected UserService? UserService { get; set; }

    protected bool _inspectionOverlayOpen = false;
    protected DataPaginatedResponse<UserItemResponse>? _users;
    protected int? _inspectingUserID;

    protected string? _successMessage;
    protected Dictionary<string, string[]> errors = [];

    protected override async Task OnInitializedAsync()
    {
        if (UserService is not null)
        {
            await GetList();
        }
    }

    protected async Task GetList(int page = 1)
    {
        if (UserService is null)
        {
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { "User Service is unavailable" } }
            };
            return;
        }

        ApiResult<DataPaginatedResponse<UserItemResponse>> result = await UserService.List(page);
        if (!result.IsSuccess)
        {
            _successMessage = null;
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { result.Error?.Details[0] ?? "An unknown error occurred." } }
            };
            return;
        }

        if (result.Data is null)
        {
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { "Something went wrong getting the response message." } }
            };
            return;
        }

        errors = [];
        _users = result.Data;
    }

    protected async Task DeleteUser(int userID)
    {
        _inspectionOverlayOpen = false;
        errors = [];
        _successMessage = null;

        if (UserService is null)
        {
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { "User Service is unavailable" } }
            };
            return;
        }

        ApiResult<SuccessfulResponse> result = await UserService.Delete(userID);
        if (!result.IsSuccess)
        {
            _successMessage = null;
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { result.Error?.Details[0] ?? "An unknown error occurred." } }
            };
            return;
        }

        if (result.Data is null)
        {
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { "Something went wrong getting the response message." } }
            };
            return;
        }

        await GetList(); // Laddar om användarlistan.

        errors = [];
        _successMessage = result.Data.Message;
    }

    protected void Inspect(int userID)
    {
        _inspectionOverlayOpen = !_inspectionOverlayOpen;
        _inspectingUserID = userID;
    }

    // Kollar om användaren kan gå till föregånede sida.
    protected bool _canGoPrevious =>
       _users is not null &&
       _users.Pagination.current_page > 1;

    // Kollar om användare gå till nästa sida.
    protected bool _canGoNext =>
        _users is not null &&
        _users.Pagination.current_page < _users.Pagination.last_visible_page;

    // Skickar användaren till föregående sida.
    protected async Task GoToPreviousPage()
    {
        if (!_canGoPrevious || _users is null) return;
        int previousPage = _users.Pagination.current_page - 1;
        await GetList(previousPage);
    }

    // Skickar användaren till nästa sida.
    protected async Task GoToNextPage()
    {
        if (!_canGoNext || _users is null) return;
        int nextPage = _users.Pagination.current_page + 1;
        await GetList(nextPage);
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}