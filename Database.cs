using System.Diagnostics;
using System;
using System.IO;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using System.Threading;
using CounterStrikeSharp.API.Modules.Utils;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using Dapper;
    
    
namespace KnifeSelector
{
    public class Database
    {
        private SqliteConnection _connection;

        public void Initialize(string directory)
        {
            _connection =
                new SqliteConnection(
                    $"Data Source={Path.Join(directory, "database.db")}");

            _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS `knifeselector` (
                `steamid` UNSIGNED BIG INT NOT NULL,
                `selected_knife` VARCHAR(32) NOT NULL DEFAULT 'weapon_knife',
                PRIMARY KEY (`steamid`));");
        }
        public bool ClientExistsInDatabase(ulong steamid)
        {
            //Server.PrintToConsole($"---------------------checking if user already exist {steamid}");
            return _connection.ExecuteScalar<int>("select count(*) from knifeselector where steamid = @steamid",
                new { steamid }) > 0;
        }
        public void AddNewClientToDatabase(CCSPlayerController player)
        {
            Server.PrintToConsole($"---------------------Adding client to database {player.SteamID}");
            _connection.Execute(@"
            INSERT INTO knifeselector (`steamid`, `selected_knife`)
	        VALUES(@steamid, 'weapon_knife')",
                new { steamid = player.SteamID });
        }


        public void SaveCurrentKnife(CCSPlayerController player, string knifedbArg)
        {

            _connection.Execute(@"
            UPDATE `knifeselector` SET `selected_knife` = @knife WHERE `steamid` = @steamid;",
                new
                {
                    steamid = player.SteamID,
                    knife = knifedbArg
                });
        }
        
        public string GetPlayerKnifeFromDB(CCSPlayerController player)
        {
            string knife = "";
            var command = _connection.CreateCommand(); //bir komut nesnesi oluşturun
            _connection.Open ();
            command.CommandText = @"
                SELECT selected_knife
                FROM knifeselector
                WHERE steamid = @steamid
            ";
            command.Parameters.AddWithValue("@steamid", player.SteamID); //steamid parametresini belirleyin
            using (var reader = command.ExecuteReader()) //veri okuyucuyu kullanın
            {
                if (reader.Read()) //okuyucu okuyabildiyse
                {
                    knife = reader.GetString(0); //ilk sütundaki veriyi alın
                }
            }
        return knife; //bıçak değişkenini döndürün
        }   
}
}