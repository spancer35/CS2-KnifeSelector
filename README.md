# CS2 Knife Selector
Lets you choose and use a knife in game with !knife command.

> [!CAUTION]
> This plugin may trigger GSLT ban or similar bans. Use it at your own risk!

## Requirements
- [CounterStrikeSharp - Min v126](https://github.com/roflmuffin/CounterStrikeSharp)

## Installation
- Download the latest release from [releases page](https://github.com/spancer35/CS2-KnifeSelector/releases)
- Extract the zip to plugins folder
- Modify **addons\counterstrikesharp\configs\plugins\KnifeSelector\KnifeSelector.json**

## Features
- Saves user knife preferences to a DB (via SQLite)
- Checks if user member of specified steam group, if not user can't use !knife (this feature can be disabled in config)
- Easy to add new knifes after new knife being introduced to game.
- Multiple languages
  
> Please not that plugin is setting **mp_t_default_melee** and **mp_ct_default_melee** to empty values to prevent some maps to blocking giving knifes at spawn

## TODOs
- Add radio menu features as soon as it's added to CS2
- Change localization method
  
## Known Issues
- Plugin is not working on Windows servers.
- Please feel free to report me issues you find. discord @huesebio

### Credits
- BMathers for his helps and optimizing the code
- Nereziel for original idea
  


> Feel free to ask me about plugin discord @huesebio

### Plugin Configs
dir: **addons/counterstrikesharp/configs/plugins/KnifeSelector/KnifeSelector.json**
```json {
{
  "server_prefix": "[Server/Clan Name]",
  "server_language": "en",
  "check_steam_group": true,
  "steam_group_id": "xxxxxxxxxxxxxxxxx", //Put your Steam GROUP's 64ID. You can find it here > https://steamcommunity.com/groups/<YOUR GROUP>/memberslistxml/?xml=1
  "knife_command": "knife",
  "knifelist_command": "knifelist",
}
```
dir: **addons/counterstrikesharp/plugins/KnifeSelector/addons/counterstrikesharp/plugins/KnifeSelector/knifenames.json**
```json
{
    "m9": [ //chat trigger i.e : !knife m9 (can be changed)
      "weapon_knife_m9_bayonet", // do not change any of these
      "M9 Bayonet" //display name (can be changed.)
    ],
    "karambit": [
      "weapon_knife_karambit",
      "Karambit"
    ],
    "NewKnifeChatCommand": [
      "weapon_knife_new_knife",
      "New Knife"
    ]
}
```
