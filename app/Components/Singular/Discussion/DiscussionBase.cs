using app.DTOs;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class DiscussionBase : ComponentBase, IDisposable
{
    [Inject]
    protected UserStateService? UserStateService { get; set; }
    [Inject]
    protected PostService? PostService { get; set; }

    [Parameter]
    public required int MalID { get; set; }

    protected Dictionary<string, string[]> _errors = [];
    protected DataPaginatedResponse<PostResponse>? _postData;
    protected ProfileResponse? _profile;
    protected PostResponse? _postUpdatingItem; // Håller reda på den item som ska ändras.
    protected PostResponse? _postDeletionItem; // Håller reda på den item som ska raderas.

    public void Dispose()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange -= StateHasChanged;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (PostService is not null)
        {
            await LoadPosts();
        }
        if (UserStateService is not null)
        {
            _profile = UserStateService.CurrentUser;
        }
    }

    // Laddar in inlägg för en specifik anime.
    protected async Task LoadPosts(int page = 1)
    {
        if (PostService is null)
        {
            _errors = new()
            {
                { "General", new[] { "PostService is unavailable." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        PostGetRequest request = new()
        {
            TargetID = MalID,
            Page = page,
            TimeZone = UserStateService?.CurrentUser?.Settings.TimeZone ?? NodaTime.DateTimeZoneProviders.Tzdb.GetSystemDefault().Id
        };

        ApiResult<DataPaginatedResponse<PostResponse>> result = await PostService.Get(request);
        if (!result.IsSuccess)
        {
            _errors = new()
            {
                { "General", new[] { result.Error?.Details[0] ?? "An unknown error occurred." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (result.Data is null)
        {
            _errors = new()
            {
                { "General", new[] { "Data is unavailable." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        _postData = result.Data;
        await InvokeAsync(StateHasChanged);
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
        if (v && _postData is not null)
        {
            await LoadPosts(_postData.Pagination.current_page);
        }
    }
}