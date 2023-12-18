using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Data.Sqlite;
using Dapper;

namespace KnifeSelector;

public class Database
{
    private SqliteConnection? _connection;
    
    public void Initialize(string directory)
    {

        _connection = new SqliteConnection($"Data Source={Path.Join(directory, "database.db")}");
        _connection.Execute("""CREATE TABLE IF NOT EXISTS `knifeselector` (`steamid` UNSIGNED BIG INT NOT NULL,`selected_knife` VARCHAR(32) NOT NULL DEFAULT 'weapon_knife', PRIMARY KEY (`steamid`));""");

    }

    public bool ClientExistsInDatabase(ulong steamid)
    {

        return _connection != null && _connection.ExecuteScalar<int>("select count(*) from knifeselector where steamid = @steamid",new { steamid }) > 0;

    }

    public void AddNewClientToDatabase(CCSPlayerController player)
    {
        Server.PrintToConsole($"---------------------Adding client to database {player.SteamID}");
        if (_connection != null)
            _connection.Execute(@"
            INSERT INTO knifeselector (`steamid`, `selected_knife`)
	        VALUES(@steamid, 'weapon_knife')", new { steamid = player.SteamID });
    }

    public void SaveCurrentKnife(CCSPlayerController player, string knifedbArg)
    {
        if (_connection != null)
            _connection.Execute("""UPDATE `knifeselector` SET `selected_knife` = @knife WHERE `steamid` = @steamid;""",
                new
                {
                    steamid = player.SteamID,
                    knife = knifedbArg
                });
    }

    public string GetPlayerKnifeFromDb(CCSPlayerController player)
    {

        string knife = "";
        var command = _connection?.CreateCommand(); //bir komut nesnesi oluşturun
        _connection?.Open();
        
        if (command != null)
        {
            command.CommandText = """SELECT selected_knife FROM knifeselector WHERE steamid = @steamid""";
            command.Parameters.AddWithValue("@steamid", player.SteamID); //steamid parametresini belirleyin
            using var reader = command.ExecuteReader();
            if (reader.Read()) knife = reader.GetString(0);
        }

        return knife;

    }
    
}   