using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace KnifeSelector
{


    public class Translations
    {
        static Dictionary<string, string> ColorDictionary = new()
        {
            {"{DEFAULT}", ChatColors.Default.ToString()},
            {"{WHITE}", ChatColors.White.ToString()},
            {"{DARKRED}", ChatColors.Darkred.ToString()},
            {"{GREEN}", ChatColors.Green.ToString()},
            {"{LIGHTYELLOW}", ChatColors.LightYellow.ToString()},
            {"{LIGHTBLUE}", ChatColors.LightBlue.ToString()},
            {"{OLIVE}", ChatColors.Olive.ToString()},
            {"{LIME}", ChatColors.Lime.ToString()},
            {"{RED}", ChatColors.Red.ToString()},
            {"{PURPLE}", ChatColors.Purple.ToString()},
            {"{GREY}", ChatColors.Grey.ToString()},
            {"{YELLOW}", ChatColors.Yellow.ToString()},
            {"{GOLD}", ChatColors.Gold.ToString()},
            {"{SILVER}", ChatColors.Silver.ToString()},
            {"{BLUE}", ChatColors.Blue.ToString()},
            {"{DARKBLUE}", ChatColors.DarkBlue.ToString()},
            {"{BLUEGREY}", ChatColors.BlueGrey.ToString()},
            {"{MAGENTA}", ChatColors.Magenta.ToString()},
            {"{LIGHTRED}", ChatColors.LightRed.ToString()},
        };
        private string _language;

        public static Dictionary<string, string> UserNotInSteamGroup { get; } = new Dictionary<string, string>()
        {
            {"en", "{GREEN} You're not member of our steam group. Please join our group then try again." },
            { "tr", "{GREEN} Grubumuza üye değilsin. Lütfen grubumuza katıl ve tekrar dene." }
        };

        public static Dictionary<string, string> UserProfileNotPublic { get; } = new Dictionary<string, string>()
        {
            {"en", "{GREEN} Your profile is not public. Please set your profile to public and try again." },
            {"tr", "{GREEN} Profilin herkese açık değil. Lütfen profilini herkese aç ve tekrar dene." }
        };

        public static Dictionary<string, string> UserCantUseCommand { get; } = new()
        {
            {"en", "{GREEN} You're not able to use this command right now." },
            { "tr", "{GREEN} Şuan bu komutu kullanamazsın."}
        };

        public static Dictionary<string, string> UnknownKnife { get; set; } = new()
        {
            {"en", "{GREEN} Invalid Knife Name \u2029Want a list? Try" },
            {"tr", "{GREEN} Bıçak bulunamadı \u2029Liste için : " }
        };
        public static Dictionary<string, string> SeeConsoleOutput { get; set; } = new()
        {
            {"en", "{GREEN} See console for the output." },
            {"tr", "{GREEN} Çıktı için konsola bak." }
        };
        public static Dictionary<string, string> SelectedKnife { get; set; } = new()
        {
            {"en", "{GREEN} Selected:" },
            {"tr", "{GREEN} Seçildi:" }
        };

        public static Dictionary<string, string> UserSpammed { get; set; } = new()
        {
            {"en", "{GREEN} Do not spam. Wait for a while to use the command again." },
            {"tr", "{GREEN} Spam yapmayın. Komutu tekrar kullanabilmek için bir süre bekleyin." }
        };


        static string ReplaceColors(string message)
        {
            foreach (var kv in ColorDictionary)
                message = message.Replace(kv.Key, kv.Value);

            return message;
        }

        public Translations(string language)
        {
            _language = language;
        }

        public string ParseMessage(Dictionary<string, string> message)
        {
            var messageStr = message.ContainsKey(_language) ? message[_language] : message["en"];
            StringBuilder builder = new();
            builder.Append(messageStr);
            return ReplaceColors(builder.ToString());
        }
    }
}