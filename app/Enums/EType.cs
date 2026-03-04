using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace app.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum EType
{
    [EnumMember(Value = "TV")]
    TV,
    [EnumMember(Value = "OVA")]
    OVA,
    [EnumMember(Value = "Movie")]
    Movie,
    [EnumMember(Value = "Special")]
    Special,
    [EnumMember(Value = "ONA")]
    ONA,
    [EnumMember(Value = "Music")]
    Music,
    [EnumMember(Value = "TV Special")]
    TVSpecial,
    [EnumMember(Value = "CM")]
    CM,
    [EnumMember(Value = "PV")]
    PV
}