using app.DTOs;
using app.Enums;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class AnimeBase : ComponentBase
{
    [Parameter]
    public required Anime Anime { get; set; } // Data.
    [Parameter]
    public EventCallback<int> OnSaveRelay { get; set; } // Relayar händelsen och tar denna animes MalID med sig.

    protected bool _isReadingMore; // Visar hela beskrivningen i en ny overlay.
    protected bool _isWatchingTrailer; // Visar trailern i en ny overlay.
    protected bool _isAddingToSchedule; // Visar ScheduleAddForm i en ny overlay.

    // Bestämmer vilken titel, engelska titeln eller default titeln.
    protected string Title()
    {
        return Anime.Titles.FirstOrDefault(title => title.Type == "English")?.Value ?? Anime.Titles.FirstOrDefault(title => title.Type == "Default")?.Value ?? string.Empty;
    }

    // Tar ut en del av beskrivningen.
    protected string Excerpt()
    {
        if (Anime.Synopsis is not null && Anime.Synopsis.Length >= 350)
        {
            string substring = Anime.Synopsis.Substring(0, 350);
            return $"{substring}...";
        }
        else
        {
            return "There's no description for this anime...";
        }
    }

    // Gör om DateComponent till DateTime. Det görs ingen säkerhetskontroll om siffrorna är korrekta.
    protected DateOnly? AiredDate(DateComponent date)
    {
        if (date.Year is not null && date.Month is not null && date.Day is not null)
        {
            return new DateOnly((int)date.Year, (int)date.Month, (int)date.Day);
        }
        else
        {
            return null;
        }
    }

    // Anger statusen.
    protected string Status()
    {
        string status = Anime.Status switch
        {
            EStatus.NotYetAired => "Upcoming",
            EStatus.CurrentlyAiring => "Airing",
            EStatus.FinishedAiring => "Finished",
            _ => "?"
        };
        return status;
    }

    // Hämtar ikonen för addschedule.
    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }
}