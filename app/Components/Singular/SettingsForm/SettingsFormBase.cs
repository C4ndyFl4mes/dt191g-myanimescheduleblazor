using app.DTOs;
using app.Services;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace app.Bases;

public class SettingsFormBase : ComponentBase, IDisposable
{
    [Inject]
    protected UserStateService? UserStateService { get; set; }
    [Inject]
    protected UserService? UserService { get; set; }
    [Inject]
    protected SessionService? SessionService { get; set; }

    protected UserSettings? _settings;
    protected IEnumerable<string> _timeZones = DateTimeZoneProviders.Tzdb.Ids;

    protected string? _successMessage;
    protected string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange += StateHasChanged;
            _settings = UserStateService.CurrentUser?.Settings;
            if (_settings is null)
            {
                if (SessionService is not null)
                {
                    ProfileResponse? profile = await SessionService.GetSessionProfile();
                    _settings = profile.Settings;
                }
            }
        }

        if (UserService is not null)
        {
            await UserService.Initialize();
        }
    }

    public void Dispose()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange -= StateHasChanged;
        }
    }

    protected async Task SaveSettings()
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

        if (_settings is null)
        {
            _errorMessage = "User settings are missing.";
            return;
        }

        // Ser till att token är tillgänglig för autentisering vid uppdatering av inställningar.
        await UserService.Initialize();

        ApiResult<UserSettings> result = await UserService.SetSettings(_settings);
        if (!result.IsSuccess)
        {
            _errorMessage = result.Error?.Details?.FirstOrDefault()
                ?? result.Error?.Title
                ?? "Failed to save settings.";
            return;
        }

        UserSettings updatedSettings = result.Data ?? _settings;
        currentUser.Settings = updatedSettings;
        UserStateService.CurrentUser = currentUser;

        if (SessionService is not null)
        {
            await SessionService.SetSessionProfile(UserStateService.CurrentUser);
        }
        else
        {
            _errorMessage = "SessionService is not available.";
        }

        _successMessage = "Successfully saved settings.";
    }
}