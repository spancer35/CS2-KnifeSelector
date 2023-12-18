using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace KnifeSelector;

public class Config : BasePluginConfig
{
    [JsonPropertyName("check_steam_group")]
    public bool CheckSteamGroup { get; set; } = true;

    [JsonPropertyName("steam_group_id")]
    public string SteamGroupId { get; set; } = "xxxxxxxxxxxxxxxxx";

    [JsonPropertyName("knife_command")]
    public string KnifeCommand { get; set; } = "knife";

    [JsonPropertyName("knifelist_command")]
    public string KnifeListCommand { get; set; } = "knifelist";

}