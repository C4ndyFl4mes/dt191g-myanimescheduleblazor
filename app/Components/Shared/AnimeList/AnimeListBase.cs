using app.DTOs;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class AnimeListBase : ComponentBase, IDisposable
{
    [Inject]
    protected JikanService? JikanService { get; set; } // Används för att hämta data från Jikan API.
    [Inject]
    protected UserStateService? UserStateService { get; set; } // Används till att veta om explicit animes ska visas eller inte.
    [Inject]
    protected ScheduleService? ScheduleService { get; set; } // Används till att veta vilka animes som är i Schedule.

    protected JikanResponse? _jikanResponse; // Responsen som innehåller pagination och data.
    protected JikanError? _jikanError; // Ett specifikt error objekt för Jikan.
    protected Dictionary<string, string[]> _errors = []; // För andra fel och myanimeschedule api fel.
    protected Anime? _inspectingAnime; // Om användaren inspekterar en anime elelr inte. Visar anime data i en overlay.
    protected bool _showExplicitAnime = false; // Om explicit animes ska visas eller inte.
    protected string _currentList = "now"; // Vilket typ av lista: "now" | "ongoing" | "upcoming".
    protected HashSet<int> _animesInSchedule = []; // Håller reda på vilka animes som finns i Schedule. HashSet används för att enkelt hantera dubletter.

    protected override async Task OnInitializedAsync()
    {
        if (JikanService is not null)
        {
            await LoadList(); // Laddar in "now" listan på sidan 1.
        }

        if (ScheduleService is not null)
        {
            await LoadSchedule(); // Laddar in Schedule data.
        }

        if (UserStateService is not null)
        {
            ProfileResponse? profile = UserStateService.CurrentUser;
            if (profile is not null)
            {
                _showExplicitAnime = profile.Settings.ShowExplicitAnime; // Hämtar ShowExplicitAnime inställning.
            }
        }
    }

    // När komponenten inte längre används avprenumereras UserStateService och JikanService cache rensas.
    public void Dispose()
    {
        if (UserStateService is not null)
        {
            UserStateService.OnChange -= StateHasChanged;
        }

        // Rensar cachning när användaren lämnar Browse sidan.
        if (JikanService is not null)
        {
            JikanService.ClearCache();
        }
    }

    // Vilken lista som är aktiv just nu.
    protected string ActiveList(string listname)
    {
        return listname == _currentList ? "golden-text underline" : "";
    }

    // Väljer vilken lista.
    protected async Task SelectList(string listname)
    {
        _currentList = listname;
        await LoadList();
    }

    // Laddar in de olika listorna.
    protected async Task LoadList(int page = 1)
    {
        _jikanError = null;

        if (JikanService is null)
        {
            _errors = new()
            {
                {"General", new[] { "JikanService is unavailable." }}
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        // En switch för att bestämma vilken endpoint/lista som kommer att laddas in.
        ApiResult<JikanResponse> result = _currentList switch
        {
            "now" => await JikanService.Now(page, _showExplicitAnime),
            "ongoing" => await JikanService.Ongoing(page, _showExplicitAnime),
            "upcoming" => await JikanService.Upcoming(page, _showExplicitAnime),
            _ => new ApiResult<JikanResponse>
            {
                IsSuccess = false,
                JikanError = new JikanError(404, "List not found", "The list you're looking for does not exist.", "", null),
                Data = null
            }
        };

        if (!result.IsSuccess)
        {
            _jikanError = result.JikanError;
            await InvokeAsync(StateHasChanged);
            return;
        }
        else
        {
            _jikanResponse = result.Data;
            _jikanError = null;
        }

        await InvokeAsync(StateHasChanged);
    }


    // Laddar in Schedule för att kunna veta vilka animes är i schedule.
    protected async Task LoadSchedule()
    {
        if (ScheduleService is null)
        {
            _errors = new()
            {
                {"General", new[] { "ScheduleService is unavailable." }}
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        ApiResult<ScheduleResponse> result = await ScheduleService.Get();
        if (!result.IsSuccess)
        {
            if (result.Error?.StatusCode == 401)
            {
                return;
            }
            _errors = new Dictionary<string, string[]>
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
                {"General", new[] { "Schedule data is unavailable." }}
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        // Lägger in alla MalIDn, hoppar över ifall den redan finns i _animesInSchedule.
        foreach(ScheduleWeekDayResponse weekday in result.Data.WeekDays)
        {
            foreach(ScheduleEntryResponse entry in weekday.ScheduleEntries)
            {
                if (!_animesInSchedule.Add(entry.MalID))
                    continue;
            }
        }
        await InvokeAsync(StateHasChanged);
    }

    // Uppdaterar UI med att den sparade animen är nu med i schedule.
    protected async Task UpdateUI(int malID)
    {
        if(_animesInSchedule.Add(malID) && _jikanResponse is not null)
            await LoadList(_jikanResponse.Pagination.current_page);
    }
}