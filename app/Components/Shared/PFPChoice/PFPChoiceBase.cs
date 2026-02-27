using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class PFPChoiceBase : ComponentBase
{
    [Parameter]
    public required string ChoiceName { get; set; }

    [Parameter]
    public required string ChoiceImageURL { get; set; } 

    [Parameter]
    public required EventCallback<string> OnSelected { get; set; }

    [Parameter]
    public required bool Selected { get; set; }

    protected string _selectedStyleClass => Selected ? "border-4" : " hover:border-2";

    protected async Task Select()
    {
        await OnSelected.InvokeAsync(ChoiceImageURL);
    }

}