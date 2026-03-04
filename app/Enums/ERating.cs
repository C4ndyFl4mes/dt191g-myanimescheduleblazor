using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace app.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ERating
{
    [EnumMember(Value = "G - All Ages")]
    G,
    [EnumMember(Value = "PG - Children")]
    PG,
    [EnumMember(Value = "PG-13 - Teens 13 or older")]
    PG13,
    [EnumMember(Value = "R - 17+ (violence & profanity)")]
    R17,
    [EnumMember(Value = "R+ - Mild Nudity")]
    RPlus,
    [EnumMember(Value = "Rx - Hentai")]
    Rx
}