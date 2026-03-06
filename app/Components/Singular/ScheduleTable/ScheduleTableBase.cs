using app.DTOs;
using app.Enums;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class ScheduleTableBase : ComponentBase
{
    [Inject]
    protected ScheduleService? ScheduleService { get; set; }

    protected ScheduleResponse? Schedule;
    protected Dictionary<string, string[]> _errors = [];
    protected ScheduleEntryResponse? _inspectingEntry;

    protected override async Task OnInitializedAsync()
    {
        await LoadSchedule();
    }

    // Laddar in schemat.
    protected async Task LoadSchedule()
    {
        _errors = [];

        if (ScheduleService is null)
        {
            _errors = new()
            {
                { "General", new[] { "ScheduleService is not available." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        ApiResult<ScheduleResponse> result = await ScheduleService.Get();
        if (!result.IsSuccess)
        {
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

        _errors = [];
        Schedule = result.Data;
        Structuring();
    }

    // Strukturerar schemat så att dagar utan animes visas som tomma.
    protected void Structuring()
    {
        if (Schedule is not null && Schedule.WeekDays is not null)
        {
            // Ser till att alla veckodagar är med, även tomma.
            List<EWeekday> allDays = Enum.GetValues(typeof(EWeekday))
                .Cast<EWeekday>()
                .ToList();

            foreach (EWeekday day in allDays)
            {
                if (!Schedule.WeekDays.Any(w => w.DayOfWeek == day))
                {
                    Schedule.WeekDays.Add(new ScheduleWeekDayResponse 
                    { 
                        DayOfWeek = day,
                        ScheduleEntries = []
                    });
                }
            }

            // Sortera för att behålla ordningen by dagarna.
            Schedule.WeekDays = Schedule.WeekDays.OrderBy(w => w.DayOfWeek).ToList();
        }
    }

    protected async Task Reload(bool v)
    {
        if (!v)
        {
            await LoadSchedule();
            _inspectingEntry = null;
        }
    }
}