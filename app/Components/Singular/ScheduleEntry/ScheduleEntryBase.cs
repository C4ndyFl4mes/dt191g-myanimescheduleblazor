using app.DTOs;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class ScheduleEntryBase : ComponentBase
{
    [Parameter]
    public required ScheduleEntryResponse Entry { get; set; }
    [Parameter]
    public EventCallback<ScheduleEntryResponse> OnInspection { get; set; }


    protected async Task Inspect()
    {
        if (OnInspection.HasDelegate)
        {
            await OnInspection.InvokeAsync(Entry);
        }
    }
    
    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}