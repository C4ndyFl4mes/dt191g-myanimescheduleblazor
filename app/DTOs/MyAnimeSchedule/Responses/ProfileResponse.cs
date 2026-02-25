namespace app.DTOs;

public record ProfileResponse
{
    public string? Username { get; set; }
    public string? Role { get; set; }
    public required UserSettings Settings { get; set; }
    public string? Token { get; set; }
}