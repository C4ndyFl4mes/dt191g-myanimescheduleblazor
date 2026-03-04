using app.DTOs;
using app.Enums;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class ScheduleAddFormBase : ComponentBase
{
    [Inject]
    protected ScheduleService? ScheduleService { get; set; } // Används för lägga till anime i Schedule.

    [Parameter]
    public required int MalID { get; set; } // Används för att kunna identifiera vilken anime det handlar om.
    [Parameter]
    public required string AnimeTitle { get; set; } // Används för titeln på overlayen.
    [Parameter]
    public EventCallback<int> OnSave { get; set; } // Används för att kunna uppdatera UI med statusen In Schedule.

    protected string _letMeDecide = "onrelease"; // Bestämmer om användaren ska ange veckodag och tid eller ifall animes ska schema läggas på release.
    protected Dictionary<string, string[]> _errors = []; // Felmeddelanden från myanimeschedule API:et eller andra generella fel som att ScheduleService inte är tillgänglig. 
    protected string[] Weekdays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]; // Används enbart för Options i CustomSelectBoxen.
    protected string? _successMessage; // Ett meddelande för när animen är lagts till i schedule.

    // Anger startvärden för _scheduleForm.
    protected ScheduleForm _scheduleForm = new()
    {
        Mal_ID = 0,
        WatchDay = "Monday",
        Time = new TimeOnly(0).AddHours(12)
    };

    protected override async Task OnInitializedAsync()
    {
        _scheduleForm.Mal_ID = MalID; // Specifierar vilken anime det handlar om.
    }

    // Sparar animen till Schedule och därefter skickar en händelse som notifierar att animen ska visa "In Schedule".
    protected async Task Save()
    {
        if (ScheduleService is not null)
        {
            _errors = [];
            _successMessage = null;

            ScheduleRequest request = new()
            {
                Mal_ID = _scheduleForm.Mal_ID,
                WatchDay = _letMeDecide == "specified" ? Enum.Parse<EWeekday>(_scheduleForm.WatchDay) : null,
                Time = _letMeDecide == "specified" ? _scheduleForm.Time : null
            };
  
            ApiResult<SuccessfulResponse> result = await ScheduleService.Post(request);

            if (!result.IsSuccess)
            {
                // Tar ut 409 felmeddelandet och göra det mer användarvänligt.
                if (result.Error?.StatusCode == 409)
                {
                    _errors = new Dictionary<string, string[]>
                    {
                        { "General", new[] { "You have already added this anime to your schedule. Check your schedule." } }
                    };
                    return;
                }

                _errors = new Dictionary<string, string[]>
                {
                    { "General", new[] { result.Error?.Details[0] ?? "An unknown error occurred." } }
                };
                return;
            }

            if (result.Data is null)
            {
                _errors = new Dictionary<string, string[]>
                {
                    { "General", new[] { "Something went wrong getting the schedule data." } }
                };
                return;
            }

            _errors = [];
            _successMessage = "Successfully saved schedule.";

            // Skickar iväg notisen.
            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync(_scheduleForm.Mal_ID);
            }
        }
    }
}