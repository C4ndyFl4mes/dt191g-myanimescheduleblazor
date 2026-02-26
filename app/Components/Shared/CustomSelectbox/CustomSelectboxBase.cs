using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class CustomSelectboxBase : ComponentBase
{
    [Parameter]
    public string Id { get; set; } = string.Empty; // ID för att koppla label till.
    [Parameter]
    public IEnumerable<string> Options { get; set; } = []; // Lista med alternativ att visa i selectboxen.
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; } // EventCallback för att meddela när värdet ändras.
    [Parameter]
    public string Value { get; set; } = string.Empty; // Det aktuella värdet av selectboxen. Kan initieras via parameter.

    protected void OnValueChanged(ChangeEventArgs e)
    {
        Value = e.Value?.ToString() ?? string.Empty;
        ValueChanged.InvokeAsync(Value);
    }
}