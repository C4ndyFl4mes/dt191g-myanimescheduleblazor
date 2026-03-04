using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace app.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ESeason
{
    [EnumMember(Value = "summer")]
    Summer,
    [EnumMember(Value = "winter")]
    Winter,
    [EnumMember(Value = "spring")]
    Spring,
    [EnumMember(Value = "fall")]
    Fall
}