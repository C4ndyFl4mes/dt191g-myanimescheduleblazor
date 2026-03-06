using app.DTOs;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class PaginationBase : ComponentBase
{
    [Parameter]
    public Pagination? Pagination { get; set; } // Pagineringsobjektet.
    [Parameter]
    public EventCallback<int> PageChanged { get; set; } // Notifierar moderkomponenten ifall sidan har ändrats.

    // Kollar om användaren kan gå till föregånede sida.
    protected bool _canGoPrevious =>
       Pagination is not null &&
       Pagination.current_page > 1;

    // Kollar om användare gå till nästa sida.
    protected bool _canGoNext =>
        Pagination is not null &&
        Pagination.current_page < Pagination.last_visible_page;

    // Skickar användaren till föregående sida.
    protected async Task GoToPreviousPage()
    {
        if (!_canGoPrevious || Pagination is null)
            return;
        int previousPage = Pagination.current_page - 1;
        if (PageChanged.HasDelegate)
        {
            await PageChanged.InvokeAsync(previousPage);
        }
    }

    // Skickar användaren till nästa sida.
    protected async Task GoToNextPage()
    {
        if (!_canGoNext || Pagination is null)
            return;
        int nextPage = Pagination.current_page + 1;
        if (PageChanged.HasDelegate)
        {
            await PageChanged.InvokeAsync(nextPage);
        }
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}