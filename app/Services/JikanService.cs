using app.DTOs;
using Flurl;
using Flurl.Http;

namespace app.Services;

public class JikanService
{
    // Cachning av data från Jikan. Det finns åtta olika kombinationer av cachningar (tre olika listor), vilket kan bli mycket.
    // Key (endpoint) prefix + (Show Explicit Anime) suffix exempel: "now || false" -> nuvarande säsong och inte tillåta explicit anime.
    private readonly Dictionary<string, List<Anime>> _itemsByKey = new(); // Cachade anime data. Kan innehålla tre olika listor för now, now?ongoing och upcoming.
    private readonly Dictionary<string, HashSet<int>> _seenIdsByKey = new(); // MAL idn på animes redan hämtade. HasSet returnerar null vid add ifall ett ID redan hämtad.
    private readonly Dictionary<string, int> _nextApiPageByKey = new(); // Nästa API sida. 
    private readonly Dictionary<string, bool> _hasMoreByKey = new(); // Håller reda på om det finns en nästa API sida som kan hämtas.

    // Rensar cache. Om ingen endpoint eller inställning angett kommer alla cachningar att rensas.
    public void ClearCache(string? endpoint = null, bool? showExplicitAnime = null)
    {
        if (endpoint is null && showExplicitAnime is null)
        {
            _itemsByKey.Clear();
            _seenIdsByKey.Clear();
            _nextApiPageByKey.Clear();
            _hasMoreByKey.Clear();
            return;
        }

        string prefix = endpoint is null ? "" : $"{endpoint}|";
        string suffix = showExplicitAnime is null ? "" : $"|{showExplicitAnime.Value}";

        // Rensar cachning för specifik key, exempelvis now | false.
        foreach (string key in _itemsByKey.Keys.Where(k => k.StartsWith(prefix) && k.EndsWith(suffix)).ToList())
        {
            _itemsByKey.Remove(key);
            _seenIdsByKey.Remove(key);
            _nextApiPageByKey.Remove(key);
            _hasMoreByKey.Remove(key);
        }
    }


    // Metoder för att skapa olika cachningar. Varje metod är sin egen lista.

    // Hämtar nuvarande säsongs anime.
    // Unika keys: "now | false" och "now | true" Betydelse för cachningskombinationer över de fyra dictionaries: 2x4 (8)
    public Task<ApiResult<JikanResponse>> Now(int localPage, bool showExplicitAnime) =>
        GetLocalPage("now", localPage, showExplicitAnime);

    // Hämtar nuvarande säsongs anime och anime som fortfarande pågår från föregående säsonger.
    // Unika keys: "now?continuing | false" och "now | true" Cachningskombinationer över de fyra dictionaries: 2x4 (8)
    public Task<ApiResult<JikanResponse>> Ongoing(int localPage, bool showExplicitAnime) =>
        GetLocalPage("now?continuing", localPage, showExplicitAnime);

    // Hämtar nästa säsongs anime.
    // Unika keys: "year/season | false" och "year/season | true" Cachningskombinationer över de fyra dictionaries: 2x4 (8)
    public Task<ApiResult<JikanResponse>> Upcoming(int localPage, bool showExplicitAnime)
    {
        var (season, year) = GetNextAnimeSeason();
        return GetLocalPage($"{year}/{season}", localPage, showExplicitAnime);
    }

    // Om jag räknat rätt så kan det vara 24 st cachningar över de fyra dictionaries. Därav sparas varje lista av animes 4 gånger men att dubletter räknas bort. 
    // Tyvärr cachas det inte över gränserna. Eftersom now?continuing också innehåller animes från now sparas mer data än vad som skulle varit mest effektivt.
    // Men det tåls att säga att showAnimeExplicit kan aldrig vara både sant och falskt under en period där användaren cachar animes från olika listor pga när användaren
    // lämnar sidan rensas cachningen i Dispose i AnimeListBase.

    // Hämtar och cachar alla unika animes som behövs för att visa den lokala sidan som består av up till 50 animes. Hämtar inte nya animes för sidor som redan har laddats in.
    public async Task<ApiResult<JikanResponse>> GetLocalPage(string endpoint, int localPage, bool showExplicitAnime, int localPageSize = 50)
    {
        // Normaliserar värdena.
        localPage = Math.Max(1, localPage); // Antingen första sidan eller nuvarande sida som antigngen är 1 eller större.
        localPageSize = Math.Max(1, localPageSize); // Antalet items per sida, 1 ifall localPageSize inte är större, normalt är det 50.

        string key = $"{endpoint}|{showExplicitAnime}"; // Skapar en key, exempelvis: now | false.

        // Ifall denna cachning inte finns sedan tidigare, skapas den.
        if (!_itemsByKey.ContainsKey(key))
        {
            _itemsByKey[key] = new List<Anime>();
            _seenIdsByKey[key] = new HashSet<int>();
            _nextApiPageByKey[key] = 1;
            _hasMoreByKey[key] = true;
        }

        // Hämtar föregående cachning.
        List<Anime> items = _itemsByKey[key]; // Data
        HashSet<int> seen = _seenIdsByKey[key]; // MAL IDn
        int neededCount = localPage * localPageSize; // Antalet animes som ska vara cachade vid tillfället. Föregående sidor + denna sida.

        // Loopen cachar animes tills antalet animes når målet av cachade animes vid tillfället.
        while (_hasMoreByKey[key] && items.Count < neededCount)
        {
            // Hämtar animes på nuvarande api sida. Endpoint är vilket lista som animes tillhör och showExplicitAnime är om explicita animes som h*ntai ska tillåtas.
            // Dock filtreras inte Ecchi eller Eroticas. Jag kan göra något åt det, men då denna applikation endast är en integration med Jikan ska jag inte påverka datan så mycket.
            int apiPage = _nextApiPageByKey[key];
            ApiResult<JikanResponse> apiResult = await Fetch(endpoint, apiPage, showExplicitAnime);

            await Task.Delay(334); // Delay som förhindrar att gå över rate limit per sekund (3req/s).

            // Avbryter genast ifall det blir något fel eller ingen data kommer.
            if (!apiResult.IsSuccess || apiResult.Data is null)
                return apiResult;

            // Lägger till unika animes i cachningen.
            foreach (Anime anime in apiResult.Data.Data)
            {
                if (seen.Add(anime.MalId))
                    items.Add(anime);
            }

            // Om det finns en nästa sida eller inte.
            _hasMoreByKey[key] =
                apiResult.Data.Pagination.current_page < apiResult.Data.Pagination.last_visible_page;

            // Ökar den nuvarande API sidan med ett.
            _nextApiPageByKey[key] = apiPage + 1;
        }

        // Tar animes för nuvarande sida genom att hoppa över föregående cachade (lokala) sidor och ta nuvarande lokala sidan.
        List<Anime> pageItems = items
            .Skip((localPage - 1) * localPageSize)
            .Take(localPageSize)
            .ToList();

        // Sidor vi vet om och sista sidan, vilket vi inte kan veta.
        int knownLocalPages = Math.Max(1, (int)Math.Ceiling(items.Count / (double)localPageSize));
        int lastVisibleLocalPage = _hasMoreByKey[key] ? knownLocalPages + 1 : knownLocalPages;
        string lastVisibleLocalPageDisplay = _hasMoreByKey[key] ? "?" : knownLocalPages.ToString();

        Pagination pagination = new()
        {
            current_page = localPage,
            last_visible_page = lastVisibleLocalPage,
            last_visible_page_display = lastVisibleLocalPageDisplay,
            has_next_page = _hasMoreByKey[key],
            items = new()
            {
                count = items.Count,
                total = items.Count,
                per_page = localPageSize
            }
        };

        return new ApiResult<JikanResponse>
        {
            IsSuccess = true,
            Data = new JikanResponse(pagination, pageItems)
        };
    }

    // Hämtar data från Jikan API.
    private async Task<ApiResult<JikanResponse>> Fetch(string endpoint, int page, bool showExplicitAnime)
    {
        try
        {
            Url url = $"https://api.jikan.moe/v4/seasons/{endpoint}"
                .SetQueryParam("page", page);

            if (!showExplicitAnime)
                url = url.SetQueryParam("sfw", "true");

            JikanResponse response = await url
                .GetJsonAsync<JikanResponse>();

            return new ApiResult<JikanResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call?.Response?.StatusCode;
            string message = statusCode == 429
                ? "Rate limited by Jikan API (429). Wait a moment and try again. (3 req/s, 60 req/min)"
                : $"Jikan request failed ({statusCode})";

            return new ApiResult<JikanResponse>
            {
                IsSuccess = false,
                JikanError = new JikanError(
                    statusCode ?? 500,
                    "error",
                    message,
                    "jikan",
                    null
                )
            };
        }
    }

    // Hjälpmetod för att räkna ut nästa anime säsong baserat på nuvarande datum.
    private (string season, int year) GetNextAnimeSeason()
    {
        var now = DateTime.UtcNow;
        int year = now.Year;
        int month = now.Month;

        string currentSeason = month switch
        {
            >= 1 and <= 3 => "winter",
            >= 4 and <= 6 => "spring",
            >= 7 and <= 9 => "summer",
            _ => "fall"
        };

        return currentSeason switch
        {
            "winter" => ("spring", year),
            "spring" => ("summer", year),
            "summer" => ("fall", year),
            "fall" => ("winter", year + 1),
            _ => throw new Exception()
        };
    }
}