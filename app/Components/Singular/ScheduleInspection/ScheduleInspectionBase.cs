using app.DTOs;
using app.Enums;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class ScheduleInspectionBase : ComponentBase
{
    [Inject]
    protected ScheduleService? ScheduleService { get; set; }

    [Parameter]
    public required ScheduleEntryResponse Entry { get; set; }

    protected string _letMeDecide = "specified";
    protected string[] Weekdays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]; // Används enbart för Options i CustomSelectBoxen.
    protected ScheduleForm _scheduleForm = new()
    {
        Mal_ID = 0,
        WatchDay = "Monday",
        Time = new TimeOnly(0).AddHours(12)
    };
    protected string? _selectedWeekday;

    protected Dictionary<string, string[]> _errors = [];
    protected string? _successMessage;

    // Initialiserar värden för _scheduleForm.
    protected override async Task OnInitializedAsync()
    {
        if (ScheduleService is not null)
        {
            _errors = [];
            _successMessage = null;

            ApiResult<ScheduleResponse> result = await ScheduleService.Get();
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
                    { "General", new[] { "Something went wrong getting schedule data." } }
                };
                await InvokeAsync(StateHasChanged);
                return;
            }

            foreach (ScheduleWeekDayResponse weekday in result.Data.WeekDays)
            {
                if (weekday.ScheduleEntries.Any(entry => entry.Id == Entry.Id))
                {
                    _scheduleForm = new()
                    {
                        Mal_ID = Entry.MalID,
                        Id = Entry.Id,
                        WatchDay = weekday.DayOfWeek.ToString(),
                        Time = TimeOnly.Parse(Entry.Time)
                    };
                    break;
                }
            }

            await InvokeAsync(StateHasChanged);
        }
    }

    // Hanterar en entry med både update eller delete.
    protected async Task Action(string action)
    {
        _errors = [];
        _successMessage = null;

        if (ScheduleService is null)
        {
            _errors = new()
            {
                { "General", new[] {"ScheduleService is not available."} }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (_scheduleForm.Id is null)
        {
            _errors = new()
            {
                { "General", new[] {"Schedule form ID is null."} }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        ApiResult<SuccessfulResponse>? result;

        if (action == "Update")
        {
            result = await ScheduleService.Put(new()
            {
                Id = (int)_scheduleForm.Id,
                WatchDay = _letMeDecide == "specified" ? Enum.Parse<EWeekday>(_scheduleForm.WatchDay) : null,
                Time = _letMeDecide == "specified" ? _scheduleForm.Time : null
            });
        }
        else if (action == "Delete")
        {
            result = await ScheduleService.Delete((int)_scheduleForm.Id);
        }
        else
        {
            _errors = new()
            {
                { "General", new[] {"Passed in wrong action string."} }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

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
                { "General", new[] { "Something went wrong getting the response message." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        _errors = [];
        _successMessage = result.Data.Message;
        await InvokeAsync(StateHasChanged);
    }
}