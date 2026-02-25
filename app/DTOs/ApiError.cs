namespace app.DTOs;

public record ApiError
{
    public required int StatusCode { get; set; }
    public required string Title { get; set; }
    public required string[] Details { get; set; }
}