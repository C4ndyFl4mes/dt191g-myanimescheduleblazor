using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class AccountBase : ComponentBase
{
    [Inject]
    protected UserStateService? UserStateService { get; set; }
    [Inject]
    protected NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        UserStateService!.OnChange += StateHasChanged;
    }

    // Skickar användaren till en sida.
    protected void GoTo(string uri)
    {
        NavigationManager!.NavigateTo(uri);
    }
}