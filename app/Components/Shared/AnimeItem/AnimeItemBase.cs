using app.DTOs;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class AnimeItemBase : ComponentBase
{
    // För denna komponent är följande egenskaper viktiga:
    // ImageURL, Titles, Genres och Status.
    [Parameter]
    public required Anime Anime { get; set; }

    [Parameter]
    public bool InSchedule { get; set; } // Används för att visa att animen är i Schedule.

    [Parameter]
    public EventCallback<Anime> OnInspection { get; set; } // Ett event som öppnar overlayen för Anime.

    // Anger titeln, engelska eller default.
    protected string Title()
    {
        return Anime.Titles.FirstOrDefault(title => title.Type == "English")?.Value ?? Anime.Titles.FirstOrDefault(title => title.Type == "Default")?.Value ?? string.Empty;
    }

    // Öppnar animen för att visa detaljerad information.
    protected async Task Inspect()
    {
        if (OnInspection.HasDelegate)
        {
            await OnInspection.InvokeAsync(Anime);
        }
    }

    // Lägger på en extra censur på hentai för att slippa se det hela tiden.
    protected string Censor()
    {
        return Anime.Genres.Any(g => g.Name == "Hentai") ? "censor" : "";
    }
}