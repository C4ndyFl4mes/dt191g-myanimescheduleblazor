using app.DTOs;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class UserInfoBase : ComponentBase, IDisposable
{
    [Inject]
    protected UserStateService? UserStateService { get; set; }
    [Inject]
    protected UserService? UserService { get; set; }

    [Parameter]
    public required string Type { get; set; }

    protected string ActivityBordered => Type == "OnlyActivities" ? "golden-border border-4 rounded-md" : "";
    
    protected UserInfoResponse? _userInfo; // Håller reda på den inhämtade informationen.
    protected PostResponse? _postUpdatingItem; // Håller reda på den item som ska ändras.
    protected PostResponse? _postDeletionItem; // Håller reda på den item som ska raderas.
    protected int _currentPage = 1;

    protected string? _errorMessage;

    // Initialiserar tjänster som UserState och UserService.
    protected override async Task OnInitializedAsync()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange += StateHasChanged;
        }

        if (UserService is not null)
        {
            await UserService.Initialize();
            await GetUserInfo();
        }
    }

    // Avslutar prenumerationen på användaren när komponenten inte längre renderas.
    public void Dispose()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange -= StateHasChanged;
        }
    }

    // Resettar items för radering eller uppdatering.
    protected void ResetUpdateDelete(bool v)
    {
        if (!v)
        {
            _postUpdatingItem = null;
            _postDeletionItem = null;
        }
    }

    // Laddar om poster efter lyckad radering eller uppdatering.
    protected async Task ReloadPosts(bool v)
    {
        if (v)
        {
            await GetUserInfo();
        }
    }

    // Kollar om användaren kan gå till föregånede sida.
    protected bool _canGoPrevious =>
       _userInfo is not null &&
       _userInfo.Activity.Pagination.current_page > 1;

    // Kollar om användare gå till nästa sida.
    protected bool _canGoNext =>
        _userInfo is not null &&
        _userInfo.Activity.Pagination.current_page < _userInfo.Activity.Pagination.last_visible_page;

    // Skickar användaren till föregående sida.
    protected async Task GoToPreviousPage()
    {
        if (!_canGoPrevious || _userInfo is null) return;
        int previousPage = _userInfo.Activity.Pagination.current_page - 1;
        await GetUserInfo(previousPage);
    }

    // Skickar användaren till nästa sida.
    protected async Task GoToNextPage()
    {
        if (!_canGoNext || _userInfo is null) return;
        int nextPage = _userInfo.Activity.Pagination.current_page + 1;
        await GetUserInfo(nextPage);
    }

    // Hämtar användarinformation.
    protected async Task GetUserInfo(int page = 1)
    {
        // Ifall tjänsterna inte är tillgängliga ges ett användarvänligt felmeddelande.
        if (UserStateService is null || UserService is null)
        {
            _errorMessage = "Service not initialized.";
            return;
        }

        ApiResult<UserInfoResponse> result = await UserService.GetInfo(new()
        {
            Page = page
        });

        if (!result.IsSuccess)
        {
            _errorMessage = result.Error?.Details?.FirstOrDefault()
                ?? result.Error?.Title
                ?? "Failed to load user acitivities.";
            return;
        }

        if (result.Data is null)
        {
            _errorMessage = "Failted to load user acitivities.";
            return;
        }

        _errorMessage = null;
        _userInfo = result.Data;
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}