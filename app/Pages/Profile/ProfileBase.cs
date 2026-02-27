using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class ProfileBase : ComponentBase, IDisposable
{
    [Inject]
    protected UserStateService? UserStateService { get; set; }

    protected bool ChangePFPOverlayIsOpen = false;

    // Initierar UserStateService.
    protected override void OnInitialized()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange += StateHasChanged;
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

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }

    protected void TogglePFPOverlay()
    {
        ChangePFPOverlayIsOpen = !ChangePFPOverlayIsOpen;
    }
}