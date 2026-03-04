namespace app.DTOs;

public record ScheduleForm
{
    public required int Mal_ID { get; set; }
    public int? Id { get; set; }
    public required string WatchDay { get; set; }
    public required TimeOnly Time { get; set; }
}