using app.DTOs;

namespace app.Services;


public class UserStateService
{
    public event Action? OnChange;

    private ProfileResponse? _currentUser; // Token är null och hanteras enbart av SessionService.
    public ProfileResponse? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            NotifyStateChanged();
        }
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrWhiteSpace(CurrentUser?.Username);
    }

    public bool IsModerator()
    {
        return CurrentUser?.Role == "Moderator";
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}