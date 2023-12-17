using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace KnifeSelector;

public class Config : BasePluginConfig
{
    [JsonPropertyName("server_prefix")]
    public string ServerPrefix { get; set; } = "CLAN OR SERVER NAME";

    [JsonPropertyName("server_language")]
    public string ServerLanguage { get; set; } = "en";

    [JsonPropertyName("check_steam_group")]
    public bool CheckSteamGroup { get; set; } = true;

    [JsonPropertyName("steam_group_id")]
    public string SteamGroupID { get; set; } = "xxxxxxxxxxxxxxxxx";

    [JsonPropertyName("knife_command")]
    public string KnifeCommand { get; set; } = "knife";

    [JsonPropertyName("knifelist_command")]
    public string KnifeListCommand { get; set; } = "knifelist";

}