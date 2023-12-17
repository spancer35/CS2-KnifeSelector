# CS2 Knife Selector
Lets you choose and use a knife in game with !knife command.

## Requirments
[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

## Features
- Saves user knife preferences to a DB (via SQLite)
- Checks if user member of specified steam group, if not user can't use !knife (this feature can be disabled in config)
- Easy to add new knifes after new knife being introduced to game.
- Multiple languages
## TODO
- Add radio menu features as soon as it's added to CS2
## Known Issues
- Plugin is not working on Windows servers.

# Special Thanks
- Abner for translations method
- Nereziel

### Plugin Configs
dir: **addons/counterstrikesharp/configs/plugins/KnifeSelector/KnifeSelector.json**
```json {
{
  "server_prefix": "[Server/Clan Name]",
  "server_language": "en",
  "check_steam_group": true,
  "steam_group_id": "xxxxxxxxxxxxxxxxx",
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
