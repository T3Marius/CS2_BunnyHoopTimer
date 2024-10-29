using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core.Translations;
using System.Reflection;
using Tomlyn;
using Tomlyn.Model;

namespace BunnyHoopTimer;

public static class Config_Config
{
    public static Cfg Config { get; set; } = new Cfg();

    public static void Load()
    {
        string assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
        string configPath = Path.Combine(Server.GameDirectory,
                "csgo",
                "addons",
                "counterstrikesharp",
                "configs",
                "plugins",
                assemblyName,
                "config.toml"
            );

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        string configText = File.ReadAllText(configPath);
        TomlTable model = Toml.ToModel(configText);

        TomlTable tagTable = (TomlTable)model["Tag"];
        Config.Tag = StringExtensions.ReplaceColorTags(tagTable["Tag"].ToString()!);

        TomlTable stTable = (TomlTable)model["Settings"];
        Config.BunnyHoopTimer = int.Parse(stTable["BunnyHoopTimer"].ToString()!);
        Config.PrintToCenterHtml = bool.Parse(stTable["PrintToCenterHtml"].ToString()!);
        Config.ShowChatMessages = bool.Parse(stTable["ShowChatMessages"].ToString()!);
    }

    public class Cfg
    {
        public string Tag { get; set; } = string.Empty;
        public int BunnyHoopTimer { get; set; } = 30;
        public bool PrintToCenterHtml { get; set; } = true;
        public bool ShowChatMessages { get; set; } = true;
    }
}