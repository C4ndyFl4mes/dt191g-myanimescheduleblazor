using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class NavigationBase : ComponentBase
{
    [Parameter]
    public bool ShowText { get; set; }

    protected string Display => ShowText ? "hidden" : "hidden md:block";
    protected string IconPosition => ShowText ? "mx-1" : "md:mx-1";

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}