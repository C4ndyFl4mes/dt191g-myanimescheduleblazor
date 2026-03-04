using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace app.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum EStatus
{
    [EnumMember(Value = "Not yet aired")]
    NotYetAired,
    [EnumMember(Value = "Currently Airing")]
    CurrentlyAiring,
    [EnumMember(Value = "Finished Airing")]
    FinishedAiring
}