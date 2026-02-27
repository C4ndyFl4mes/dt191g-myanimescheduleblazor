using app.DTOs;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class ChangePFPFormBase : ComponentBase, IDisposable
{
    [Inject]
    protected UserStateService? UserStateService { get; set; }
    [Inject]
    protected SessionService? SessionService { get; set; }
    [Inject]
    protected UserService? UserService { get; set; }

    protected string? _currentProfilePicture;
    protected string? _successMessage;
    protected string? _errorMessage;

    // Initialiserar tjänster som UserState och UserService.
    protected override async Task OnInitializedAsync()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange += StateHasChanged;
            _currentProfilePicture = UserStateService.CurrentUser?.Settings?.ProfileImageURL;
        }

        if (UserService is not null)
        {
            await UserService.Initialize();
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

    // Ändrar profilbilden.
    protected async Task ChangePFP()
    {
        // Resettar meddelanden.
        _errorMessage = null;
        _successMessage = null;

        // Ifall tjänsterna inte är tillgängliga ges ett användarvänligt felmeddelande.
        if (UserStateService is null || UserService is null)
        {
            _errorMessage = "Service not initialized.";
            return;
        }

        // Ser till att användaren är inladdad.
        ProfileResponse? currentUser = UserStateService.CurrentUser;
        if (currentUser is null)
        {
            _errorMessage = "User not loaded.";
            return;
        }

        if (currentUser.Settings is null)
        {
            _errorMessage = "User settings are missing.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_currentProfilePicture))
        {
            _errorMessage = "Please select a profile picture.";
            return;
        }

        // Ser till att token är tillgänglig för autentisering vid uppdatering av inställningar.
        await UserService.Initialize();

        UserSettings settings = currentUser.Settings;
        settings.ProfileImageURL = _currentProfilePicture; // Uppdaterar profilbilden.

        ApiResult<UserSettings> result = await UserService.SetSettings(settings);
        if (!result.IsSuccess)
        {
            _errorMessage = result.Error?.Details?.FirstOrDefault()
                ?? result.Error?.Title
                ?? "Failed to save profile picture.";
            return;
        }

        UserSettings updatedSettings = result.Data ?? settings;
        currentUser.Settings = updatedSettings;
        UserStateService.CurrentUser = currentUser;

        if (SessionService is not null)
        {
            await SessionService.SetSessionProfile(UserStateService.CurrentUser);
        } else
        {
            _errorMessage = "SessionService is not available.";
        }

        _successMessage = "Successfully changed profile picture.";
    }

    protected static string SelectPFP(string pfp)
    {
        return $"SelectablePFPs/{pfp}.webp";
    }
}