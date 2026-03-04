using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class CustomRadioBase : ComponentBase
{
	[Parameter]
	public string Id { get; set; } = string.Empty;
	[Parameter]
	public string Name { get; set; } = string.Empty;
	[Parameter]
	public string OptionValue { get; set; } = string.Empty;
	[Parameter]
	public EventCallback<string> ValueChanged { get; set; }
	[Parameter]
	public string Value { get; set; } = string.Empty;
	[Parameter]
	public bool Disabled { get; set; } = false;

	protected bool IsChecked => string.Equals(Value, OptionValue, StringComparison.Ordinal); // Om värdena matchar är visas det att den är vald.

	protected async Task SelectValue()
	{
        
		if (Disabled || IsChecked)
		{
			return;
		}

		Value = OptionValue;
		await ValueChanged.InvokeAsync(Value);
	}
}
