using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class SidebarBase : ComponentBase
{
    protected bool collapseSidebar = true;

    protected string WidthClass => collapseSidebar ? "w-16" : "w-16 md:w-48";

    protected void ToggleSidebar()
    {
        collapseSidebar = !collapseSidebar;
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}