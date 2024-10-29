using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using static CounterStrikeSharp.API.Core.Listeners;
using static BunnyHoopTimer.Config_Config;
using static BunnyHoopTimer.Helper;
using System;

namespace BunnyHoopTimer;

public class BunnyHoopTimer : BasePlugin
{
    public override string ModuleAuthor => "T3Marius";
    public override string ModuleName => "BunnyHoop Timer";
    public override string ModuleVersion => "1.0";
    public override string ModuleDescription => "Enable bunnyhop after the timer ends on round start.";

    private int remainingCooldown;
    private bool isCountdownActive = false;
    private DateTime lastUpdateTime;

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        RegisterListener<OnTick>(OnTick);

        Config_Config.Load();
    }

    public void OnTick()
    {
        if (!isCountdownActive) return;

        if ((DateTime.Now - lastUpdateTime).TotalSeconds >= 1)
        {
            remainingCooldown = Math.Max(0, remainingCooldown - 1);
            lastUpdateTime = DateTime.Now;
        }

        string countdownMessage = Localizer["bunnyhoop_disabled_center", remainingCooldown];

        foreach (var player in GetPlayersController(true))
        {
            if (Config.PrintToCenterHtml)
            {
                player.PrintToCenterHtml(countdownMessage);
            }
        }

        if (remainingCooldown <= 0)
        {
            EnableBunnyHoop();
            isCountdownActive = false;
        }
    }
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!.WarmupPeriod)
            return HookResult.Continue;

        DisableBunnyHoop();
        remainingCooldown = Config.BunnyHoopTimer;
        isCountdownActive = true;
        lastUpdateTime = DateTime.Now;

        return HookResult.Continue;
    }

    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        isCountdownActive = false;
        return HookResult.Continue;
    }

    public void EnableBunnyHoop()
    {
        Server.ExecuteCommand("sv_autobunnyhopping 1");
        if (!Config.ShowChatMessages) return;
        Server.PrintToChatAll(Config.Tag + Localizer["bunnyhoop_enabled"]);
    }

    public void DisableBunnyHoop()
    {
        Server.ExecuteCommand("sv_autobunnyhopping 0");
        if (!Config.ShowChatMessages) return;
        Server.PrintToChatAll(Config.Tag + Localizer["bunnyhoop_disabled", Config.BunnyHoopTimer]);
    }
}
