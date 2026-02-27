using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class OverlayBase : ComponentBase
{
    [Parameter]
    public bool IsOpen { get; set; } = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback<bool> Changed { get; set; }

    protected async Task Close()
    {
        IsOpen = false;
        await Changed.InvokeAsync(false);
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}