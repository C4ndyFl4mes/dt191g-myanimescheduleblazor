using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class CustomCheckboxBase : ComponentBase
{
    [Parameter]
    public string Id { get; set; } = string.Empty; // ID för att koppla label till.
    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; } // EventCallback för att meddela när värdet ändras.
    [Parameter]
    public bool Value { get; set; } = false; // Det aktuella värdet av checkboxen. Kan initieras via parameter.

    protected void ToggleValue()
    {
        Value = !Value;
        ValueChanged.InvokeAsync(Value);
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}