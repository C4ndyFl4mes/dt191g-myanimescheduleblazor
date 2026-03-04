using System.Text.Json.Serialization;

public sealed record JikanError(
    [property: JsonPropertyName("status")]
    int Status,
    
    [property: JsonPropertyName("type")]
    string Type,

    [property: JsonPropertyName("message")]
    string Message,

    [property: JsonPropertyName("error")]
    string Error,

    [property: JsonPropertyName("report_url")]
    string? ReportUrl
);