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
    [Parameter]
    public int? UserID { get; set; }
    [Parameter]
    public EventCallback<int> OnDelete { get; set; }

    protected string ActivityBordered => Type == "OnlyActivities" ? "golden-border border-4 rounded-md" : "";
    protected string ExtraWide => Type == "Complete" ? "w-250 max-w-full" : "";

    protected UserInfoResponse? _userInfo; // Håller reda på den inhämtade informationen.
    protected PostResponse? _postUpdatingItem; // Håller reda på den item som ska ändras.
    protected PostResponse? _postDeletionItem; // Håller reda på den item som ska raderas.

    protected Dictionary<string, string[]> errors = [];

    // Initialiserar tjänster som UserState och UserService.
    protected override async Task OnInitializedAsync()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange += StateHasChanged;
        }

        if (UserService is not null)
        {
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
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { "User Service is unavailable." } }
            };
            return;
        }

        ApiResult<UserInfoResponse> result = await UserService.GetInfo(new()
        {
            TargetID = UserID,
            Page = page
        });

        if (!result.IsSuccess)
        {
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { result.Error?.Details[0] ?? "An unknown error occurred while getting user info." } }
            };
            return;
        }

        if (result.Data is null)
        {
            errors = new Dictionary<string, string[]>
            {
                { "General", new[] { "Unable to get the user data." } }
            };
            return;
        }

        errors = [];
        _userInfo = result.Data;
    }

    protected async Task SendDeleteRequest()
    {
        if (OnDelete.HasDelegate && UserID is not null)
        {
            await OnDelete.InvokeAsync((int)UserID); // Eftersom UserID kan vara null kontrolleras detta innan.
        }
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}