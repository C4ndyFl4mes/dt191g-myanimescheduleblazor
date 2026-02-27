using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class HeaderBase : ComponentBase
{
    [Inject]
    protected SessionService? SessionService { get; set; }
    [Inject]
    protected UserStateService? UserStateService { get; set; }
    protected string _title = "My Anime Schedule";
    protected string? _username;

    protected override void OnInitialized()
    {
        UserStateService!.OnChange += StateHasChanged;
    }

    protected string GetUsername()
    {
        return UserStateService?.CurrentUser?.Username ?? "Guest";
    }
}