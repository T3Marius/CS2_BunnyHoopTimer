using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API;

namespace BunnyHoopTimer;
public class Helper
{
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        var playerList = Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => p != null && p.IsValid &&
                        (IncludeBots || (!p.IsBot && !p.IsHLTV)) &&
                        p.Connected == PlayerConnectedState.PlayerConnected &&
                        ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
                        (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) ||
                        (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();

        return playerList;
    }
}