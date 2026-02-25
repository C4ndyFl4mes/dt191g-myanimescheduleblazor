using app.Enums;

namespace app.DTOs;

public record ScheduleRequest
{
    public required int Mal_ID { get; set; }
    public EWeekday? WatchDay { get; set; }
    public TimeOnly? Time { get; set; }
}