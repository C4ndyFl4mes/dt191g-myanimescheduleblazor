using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class NavigationBase : ComponentBase
{
    [Inject]
    protected UserStateService? UserStateService { get; set; }

    [Parameter]
    public bool ShowText { get; set; }

    protected string Display => ShowText ? "hidden" : "hidden md:block";
    protected string IconPosition => ShowText ? "mx-1" : "md:mx-1";

    protected override async Task OnInitializedAsync()
    {
        UserStateService!.OnChange += StateHasChanged;
    }

    // protected bool IsLoggedIn()
    // {
       
    // }
    // protected bool IsModerator()
    // {
        
    // }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}