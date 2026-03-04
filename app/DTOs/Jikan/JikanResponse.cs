using System.Text.Json.Serialization;

namespace app.DTOs;

public record JikanResponse(
    [property: JsonPropertyName("pagination")]
    Pagination Pagination,

    [property: JsonPropertyName("data")]
    IReadOnlyList<Anime> Data
);