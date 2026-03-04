using System.Text.Json.Serialization;

namespace app.DTOs;


public record AnimeImages(
    [property: JsonPropertyName("jpg")]
    AnimeImageSet Jpg,
    [property: JsonPropertyName("webp")]
    AnimeImageSet Webp
);

public record AnimeImageSet(
    [property: JsonPropertyName("image_url")]
    string? ImageUrl,
    [property: JsonPropertyName("small_image_url")]
    string? SmallImageUrl,
    [property: JsonPropertyName("large_image_url")]
    string? LargeImageUrl
);

public record TrailerBase(
    [property: JsonPropertyName("youtube_id")]
    string? YoutubeId,
    [property: JsonPropertyName("url")]
    string? Url,
    [property: JsonPropertyName("embed_url")]
    string? EmbedUrl
);

public record Title(
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("title")]
    string Value
);

public record DateRange(
    [property: JsonPropertyName("from")]
    string? From,
    [property: JsonPropertyName("to")]
    string? To,
    [property: JsonPropertyName("prop")]
    DateProp Prop
);

public record DateProp(
    [property: JsonPropertyName("from")]
    DateComponent? From,
    [property: JsonPropertyName("to")]
    DateComponent? To
);

public record DateComponent(
    [property: JsonPropertyName("day")]
    int? Day,
    [property: JsonPropertyName("month")]
    int? Month,
    [property: JsonPropertyName("year")]
    int? Year
);

public record Broadcast(
    [property: JsonPropertyName("day")]
    string? Day,
    [property: JsonPropertyName("time")]
    string? Time,
    [property: JsonPropertyName("timezone")]
    string? Timezone,
    [property: JsonPropertyName("string")]
    string? Raw
);

public record MalUrl(
    [property: JsonPropertyName("mal_id")]
    int MalId,
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("url")]
    string Url
);