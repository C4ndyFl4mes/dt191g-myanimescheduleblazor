namespace app.DTOs;

public record RawApiError
{
    public required string Type { get; set; }
    public required string Title { get; set; }
    public string? Detail { get; set; }
}