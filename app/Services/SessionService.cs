using app.DTOs;
using Blazored.SessionStorage;

namespace app.Services;

public class SessionService(ISessionStorageService _sessionStorageService)
{
    private const string KEY = "user";
    
    // GetSessionProfile hämtar användarens profil från session storage. Om ingen profil finns, returneras en standardprofil.
    public async Task<ProfileResponse> GetSessionProfile()
    {
        ProfileResponse? profile = await _sessionStorageService.GetItemAsync<ProfileResponse>(KEY) ?? new()
        {
            Username = "Not logged in",
            Role = "Guest",
            Settings = new()
            {
                ShowExplicitAnime = false,
                AllowReminders = false,
                TimeZone = "Europe/Stockholm"
            }
        };
        
        return profile;
    }

    // SetSessionProfile sparar användarens profil i session storage.
    public async Task SetSessionProfile(ProfileResponse profile)
    {
        await _sessionStorageService.SetItemAsync(KEY, profile);
    }
}