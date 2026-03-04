using System.Text.Json.Serialization;
using app.Enums;

namespace app.DTOs;

public record Anime(
    [property: JsonPropertyName("mal_id")]
    int MalId,

    [property: JsonPropertyName("url")]
    string Url,

    [property: JsonPropertyName("images")]
    AnimeImages Images,

    [property: JsonPropertyName("trailer")]
    TrailerBase Trailer,

    [property: JsonPropertyName("approved")]
    bool Approved,

    [property: JsonPropertyName("titles")]
    IReadOnlyList<Title> Titles,

    [property: JsonPropertyName("type")]
    EType? Type,

    [property: JsonPropertyName("source")]
    string? Source,

    [property: JsonPropertyName("episodes")]
    int? Episodes,

    [property: JsonPropertyName("status")]
    EStatus? Status,

    [property: JsonPropertyName("airing")]
    bool Airing,

    [property: JsonPropertyName("aired")]
    DateRange Aired,

    [property: JsonPropertyName("duration")]
    string? Duration,

    [property: JsonPropertyName("rating")]
    ERating? Rating,

    [property: JsonPropertyName("score")]
    double? Score,

    [property: JsonPropertyName("scored_by")]
    int? ScoredBy,

    [property: JsonPropertyName("rank")]
    int? Rank,

    [property: JsonPropertyName("popularity")]
    int? Popularity,

    [property: JsonPropertyName("members")]
    int? Members,

    [property: JsonPropertyName("favorites")]
    int? Favorites,

    [property: JsonPropertyName("synopsis")]
    string? Synopsis,

    [property: JsonPropertyName("background")]
    string? Background,

    [property: JsonPropertyName("season")]
    ESeason? Season,

    [property: JsonPropertyName("year")]
    int? Year,

    [property: JsonPropertyName("broadcast")]
    Broadcast Broadcast,

    [property: JsonPropertyName("producers")]
    IReadOnlyList<MalUrl> Producers,

    [property: JsonPropertyName("licensors")]
    IReadOnlyList<MalUrl> Licensors,

    [property: JsonPropertyName("studios")]
    IReadOnlyList<MalUrl> Studios,

    [property: JsonPropertyName("genres")]
    IReadOnlyList<MalUrl> Genres,

    [property: JsonPropertyName("explicit_genres")]
    IReadOnlyList<MalUrl> ExplicitGenres,

    [property: JsonPropertyName("themes")]
    IReadOnlyList<MalUrl> Themes,

    [property: JsonPropertyName("demographics")]
    IReadOnlyList<MalUrl> Demographics
);