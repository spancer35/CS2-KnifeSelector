using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
﻿using System.Globalization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Cvars;
using Newtonsoft.Json.Linq;


namespace KnifeSelector;


[MinimumApiVersion(126)]
public class KnifeSelector : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName  => "Knife Selector";
    public override string ModuleAuthor  => "spancer";
    public override string ModuleDescription  => "Allows players to select and use knifes.";
    public override string ModuleVersion  => "1.1.0";
    private Database _database;
    private DateTime[] commandCooldown = new DateTime[Server.MaxPlayers];
    public Config Config { get; set; } = new();
    public void OnConfigParsed(Config config)
    {
        this.Config = config;
    }

    public override void Load(bool hotReload)
    
    {
        _database = new Database();
        _database.Initialize(ModuleDirectory);

		RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
		RegisterListener<Listeners.OnMapStart>(OnMapStart);
		RegisterEventHandler<EventRoundStart>(OnRoundStart, HookMode.Pre);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd, HookMode.Pre);
        RegisterEventHandler<EventRoundAnnounceWarmup>(OnRoundAnnounceWarmup, HookMode.Pre);
        RegisterListener<Listeners.OnClientConnect>(Listener_OnClientConnectHandler);
        RegisterListener<Listeners.OnClientDisconnect>(Listener_OnClientDisconnectHandler);
        RegisterConsoleCommands();
    }
	public override void Unload(bool hotReload)
	{
		base.Unload(hotReload);
	}
    private void RegisterConsoleCommands()
    {
        AddCommand($"css_{Config.KnifeCommand}", "Gives player the desired knife that exist in game.", [CommandHelper(minArgs: 1, usage: "<knife name>", whoCanExecute: CommandUsage.CLIENT_ONLY)] (player, info) => { if (player == null) return; KnifeCmd(player, info); } );
        AddCommand($"css_{Config.KnifeListCommand}", "Prints the console which knifes are available.", [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)] (player, info) => { if (player == null) return; KnifeListCmd(player, info); } );
    }
    private HookResult OnRoundAnnounceWarmup(EventRoundAnnounceWarmup @event, GameEventInfo info)
    {
        NativeAPI.IssueServerCommand("mp_t_default_melee \"\"");
        NativeAPI.IssueServerCommand("mp_ct_default_melee \"\"");
        float warmupTime = ConVar.Find("mp_warmuptime").GetPrimitiveValue<float>();
        AddTimer(warmupTime, () =>
		{
        NativeAPI.IssueServerCommand("mp_t_default_melee \"\"");
        NativeAPI.IssueServerCommand("mp_ct_default_melee \"\"");
        });
        return HookResult.Continue;
    }

    [GameEventHandler]
    private void Listener_OnClientConnectHandler(int playerSlot, string name, string ipAddress)
    {
        AddTimer(2.0f, () =>
		{
        CCSPlayerController player = Utilities.GetPlayerFromSlot(playerSlot);
            if (!player.IsValid || player.IsBot) return;
            if (player == null) return;

            if (!_database.ClientExistsInDatabase(player.SteamID))
            {
                _database.AddNewClientToDatabase(player);
            }
        });
    }

    public void Listener_OnClientDisconnectHandler(int playerSlot)
    {
        CCSPlayerController player = Utilities.GetPlayerFromSlot(playerSlot);

        if (player.IsBot || !player.IsValid)
        {
            return;
        }
        // No bots, invalid clients or non-existent clients.
        if (!player.IsValid || player.IsBot) return;
    }

	private void OnMapStart(string mapName)
	{
		AddTimer(2.0f, () =>
		{
			NativeAPI.IssueServerCommand("mp_t_default_melee \"\"");
			NativeAPI.IssueServerCommand("mp_ct_default_melee \"\"");

		});

	}

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
	{

		NativeAPI.IssueServerCommand("mp_t_default_melee \"\"");
		NativeAPI.IssueServerCommand("mp_ct_default_melee \"\"");

		return HookResult.Continue;
	}

    [GameEventHandler]
    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        NativeAPI.IssueServerCommand("mp_t_default_melee \"\"");
		NativeAPI.IssueServerCommand("mp_ct_default_melee \"\"");
        return HookResult.Continue;
    }

	private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
	{
		var player = @event.Userid;       
		if(!player.IsValid || !player.PlayerPawn.IsValid || player.IsBot) return HookResult.Continue;       
        
        string knife = _database.GetPlayerKnifeFromDB(player);
        GiveKnifeToPlayerOnSpawn(player, knife);
        return HookResult.Continue;
    }
    public async Task<bool> IsUserGroupMember(ulong steamid)
    {
        CCSPlayerController? player = Utilities.GetPlayers()
        .Where(p => p.SteamID == steamid)
        .FirstOrDefault();
        if (steamid == null) return false;
        using (HttpClient client = new HttpClient())
        {
            // get the current time as a timestamp
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            // append the timestamp to the URL as a query parameter
            string url = $"https://steamcommunity.com/profiles/{steamid}/?xml=1&ts={timestamp}";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // get a stream from the URL
                        Stream stream = await client.GetStreamAsync(url);
                        // load the XML document from the stream
                        XDocument doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
                        // XPath ile bir düğüm seç
                        XElement node = doc.XPathSelectElement($"/profile/groups/group[groupID64=\"{Config.SteamGroupID}\"]");
                        XElement node1 = doc.XPathSelectElement("/profile/privacyState");
                        if (node1.Value == "public")
                        {
                            if (node != null)
                            {
                                return true;
                            }
                            else
                            {
                                Server.NextFrame(() => {player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.UserNotInSteamGroup"]}");
                                });
                                return false;
                            }
                        }else{
                            Server.NextFrame(() => {player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.UserProfileNotPublic"]}");
                            });
                            return false;
                        }
                }
                catch (XmlException ex)
                {
                    Server.NextFrame(() => {Server.PrintToConsole("XML failed to load: " + ex.Message);
                    });
                    return false;
                }
            }else{
            return false;
            }
        }
        
    }

    public bool KnifeExist(string knife)
    {
        string json = File.ReadAllText(@$"{ModuleDirectory}/knifenames.json");
        JToken token = JToken.Parse (json);
        JToken value = token.SelectToken(knife);
        if (value != null)
        {
            return true;
        }
        else
        {
            return false;

        }
    }

    
    public async void KnifeCmd(CCSPlayerController? player, CommandInfo command)
    {
        string knife = command.ArgByIndex(1);
        int playerIndex = (int)player!.Index;
        if (commandCooldown != null && DateTime.UtcNow >= commandCooldown[playerIndex].AddSeconds(5))
        {
            commandCooldown[playerIndex] = DateTime.UtcNow;
            if (player == null || !player.IsValid || !player.PawnIsAlive)
            {
                player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.UserCantUseCommand"]}");
                
            }else{
                if (KnifeExist(knife))
                {
                    
                    if (Config.CheckSteamGroup)
                    {                        
                        if (await IsUserGroupMember(player.SteamID))
                        {
                        Server.NextFrame(() => {           
                        GiveKnifeToPlayer(player, knife);
                        });
                        }
                    }else{
                    GiveKnifeToPlayer(player, knife); 
                    }
                }else{
                player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.UnknownKnife", Config.KnifeListCommand]}");
                }
            
            }
        
        }else{
            player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.UserSpammed"]}");
        }
    }

    public void KnifeListCmd(CCSPlayerController? player, CommandInfo command)
    {
    string json = File.ReadAllText(@$"{ModuleDirectory}/knifenames.json");
    JToken token = JToken.Parse(json);
        if (player == null || !player.IsValid)
        {
            player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.UserCantUseCommand"]}");
            
        }else{
            player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.SeeConsoleOutput"]}");
            player.PrintToConsole(new string ('\u2029', 15));
            player.PrintToConsole(new string ('*', 30));
            player.PrintToConsole($"{Localizer["ks.ConsoleInstructor"]} !{Config.KnifeCommand} karambit");
            player.PrintToConsole(new string ('*', 30));
            foreach (JProperty property in token)
            {
            string key = property.Name;
            JArray value = (JArray)property.Value;
            // Anahtarı ve değeri konsola yazdırın
            player.PrintToConsole($"for ★{value[1]}     use !{Config.KnifeCommand} {key}");
            }
        }
        player.PrintToConsole(new string ('*', 30));
        player.PrintToConsole("Plugin created by spancer");
        player.PrintToConsole("https://github.com/spancer35");
        player.PrintToConsole(new string ('*', 30));
    }
    private void RemoveKnifeFromPlayer(CCSPlayerController player)
    {
        var weapons = player.PlayerPawn.Value.WeaponServices!.MyWeapons;
        foreach (var weapon in weapons)
        {
            if (weapon.IsValid && weapon.Value.IsValid)
            {
                //if (weapon.Value.AttributeManager.Item.ItemDefinitionIndex == 42 || weapon.Value.AttributeManager.Item.ItemDefinitionIndex == 59)
                if (weapon.Value.DesignerName.Contains("knife"))
                {
                    weapon.Value.Remove();
                    return;
                }
            }

        }
    }


    public void GiveKnifeToPlayer(CCSPlayerController player, string knifeArg)
    {
        string json = File.ReadAllText(@$"{ModuleDirectory}/knifenames.json");
        JToken token = JToken.Parse (json);
        JToken value = token.SelectToken(knifeArg);

        if (knifeArg == "default") {
            RemoveKnifeFromPlayer(player);
            player.GiveNamedItem((CsTeam)player.TeamNum == CsTeam.Terrorist ? "weapon_knife_t" : "weapon_knife");
        }else{
            if (KnifeExist(knifeArg))
            {
                string weapon_KnifeName = value[0].ToString();
                string weapon_DisplayName = value[1].ToString();
                player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.SelectedKnife", weapon_DisplayName]}");    
                RemoveKnifeFromPlayer(player);
                player.GiveNamedItem(weapon_KnifeName);
                _database.SaveCurrentKnife(player, weapon_KnifeName);
            }
            else
            {
                player.PrintToChat($" {Localizer["ks.ServerPrefix"]} {Localizer["ks.UnknownKnife", Config.KnifeListCommand]}");
            }
        }
    }

    private void GiveKnifeToPlayerOnSpawn(CCSPlayerController player, string knifeName)
    {
            RemoveKnifeFromPlayer(player);
            player.GiveNamedItem(knifeName);
    }
}