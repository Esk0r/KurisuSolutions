#region Copyright � 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Summoners/CoreSum.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    public class CoreSum
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual string[] ExtraNames { get; set; }
        internal virtual float Range { get; set; }
        internal virtual int Duration { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent { get { return Menu.Parent; } }
        public SpellSlot Slot { get { return Player.GetSpellSlot(Name); } }
        public Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public CoreSum CreateMenu(Menu root)
        {
            try
            {
                Menu = new Menu(DisplayName, "m" + Name);

                if (!Name.Contains("smite") && !Name.Contains("teleport"))
                    Menu.AddItem(new MenuItem("use" + Name, "Use " + DisplayName)).SetValue(true).Permashow();
 
                if (Name == "summonersnowball")
                    Activator.UseEnemyMenu = true;

                if (Name == "summonerheal")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("selflowhp" + Name + "pct", "Use on Hero HP % <=")).SetValue(new Slider(20));
                    Menu.AddItem(new MenuItem("selfmuchhp" + Name + "pct", "Use on Hero Dmg Dealt % >=")).SetValue(new Slider(45));
                    Menu.AddItem(new MenuItem("use" + Name + "tower", "Include Tower Damage")).SetValue(true);
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }, 1));
                }

                if (Name == "summonerboost")
                {
                    Activator.UseAllyMenu = true;
                    var ccmenu = new Menu(DisplayName + " Buff Types", DisplayName.ToLower() + "cdeb");
                    ccmenu.AddItem(new MenuItem(Name + "cignote", "Ignite")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cexhaust", "Exhaust")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cstun", "Stuns")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "ccharm", "Charms")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "ctaunt", "Taunts")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cfear", "Fears")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cflee", "Flee")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "csnare", "Snares")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "csilence", "Silences")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "csupp", "Supression")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cpolymorph", "Polymorphs")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cblind", "Blinds")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cslow", "Slows")).SetValue(false);
                    ccmenu.AddItem(new MenuItem(Name + "cpoison", "Poisons")).SetValue(true);
                    Menu.AddSubMenu(ccmenu);

                    Menu.AddItem(new MenuItem("use" + Name + "number", "Min Buffs to Use")).SetValue(new Slider(1, 1, 5))
                        .SetTooltip("Will Only " + DisplayName + " if Your Buff Count is >= Value");
                    Menu.AddItem(new MenuItem("use" + Name + "time", "Min Durration to Use")).SetValue(new Slider(2, 1, 5))
                        .SetTooltip("Will Only " + DisplayName + " if the Buff is >= Value (Seconds)");
                    Menu.AddItem(new MenuItem("use" + Name + "od", "Use for Dangerous Only")).SetValue(false);
                    Menu.AddItem(new MenuItem("use" + Name + "delay", "Activation Delay (in ms)")).SetValue(new Slider(150, 0, 500));
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
                }

                if (Name == "summonerdot")
                {
                    Activator.UseEnemyMenu = true;

                    switch (Player.ChampionName)
                    {
                        case "Ahri":
                            Menu.AddItem(new MenuItem("ii" + Player.ChampionName, Player.ChampionName + ": Check Charm"))
                                .SetValue(false).SetTooltip("Only ignite if target is charmed?");
                            break;
                        case "Cassiopeia":
                            Menu.AddItem(new MenuItem("ii" + Player.ChampionName, Player.ChampionName + ": Check Poison"))
                                .SetValue(false).SetTooltip("Only ignite if target is poisoned?");
                            break;
                        case "Diana":
                            Menu.AddItem(new MenuItem("ii" + Player.ChampionName, Player.ChampionName + ": Check Moonlight?"))
                                .SetValue(false).SetTooltip("Only ignite if target has moonlight debuff?");
                            break;
                    }

                    Menu.AddItem(new MenuItem("itu", "Dont Ignite Near Turret")).SetValue(true);
                    Menu.AddItem(new MenuItem("igtu", "-> Ignore after Level")).SetValue(new Slider(11, 1, 18));
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: "))
                        .SetValue(new StringList(new[] { "Killsteal", "Combo" }, 1));
                }

                if (Name == "summonermana")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("selflowmp" + Name + "pct", "Minimum Mana % <=")).SetValue(new Slider(40));        
                }

                if (Name == "summonerbarrier")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("selflowhp" + Name + "pct", "Use on Hero HP % <=")).SetValue(new Slider(20));
                    Menu.AddItem(new MenuItem("selfmuchhp" + Name + "pct", "Use on Hero Dmg Dealt % >=")).SetValue(new Slider(45));
                    Menu.AddItem(new MenuItem("use" + Name + "ulti", "Use on Dangerous (Ultimates Only)")).SetValue(true);
                    Menu.AddItem(new MenuItem("use" + Name + "tower", "Include Tower Damage")).SetValue(true);
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }, 1));           
                }

                if (Name == "summonerexhaust")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.AddItem(new MenuItem("a" + Name + "pct", "Exhaust on ally HP %")).SetValue(new Slider(35));
                    Menu.AddItem(new MenuItem("e" + Name + "pct", "Exhaust on enemy HP %")).SetValue(new Slider(45));
                    Menu.AddItem(new MenuItem("f" + Name, "Force Exhaust (Dangerous)"))
                        .SetValue(true).SetTooltip("Will force exhaust dangerous spells ignoring HP% ");
                    Menu.AddItem(new MenuItem("use" + Name + "ulti", "Use on Dangerous (Utimates Only)"))
                        .SetValue(true).SetTooltip("Or spells with \"Force Exhaust\"");
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
                }

                if (Name == "summonersmite")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.AddItem(new MenuItem("usesmite", "Use Smite")).SetValue(new KeyBind('M', KeyBindType.Toggle, true)).Permashow();
                    Menu.AddItem(new MenuItem("smiteskill", "Smite + Ability")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitesmall", "Smite Small Camps")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitelarge", "Smite Large Camps")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitesuper", "Smite Epic Camps")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitemode", "Smite Enemies: "))
                        .SetValue(new StringList(new[] { "Killsteal", "Combo", "Nope" }, 1));
                    Menu.AddItem(new MenuItem("savesmite", "Save a Smite Charge")
                        .SetValue(true).SetTooltip("Will only combo smite if Ammo > 1"));
                }

                if (Name == "summonerteleport")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("telesep", "Beta (Pings are Local)"));
                    Menu.AddItem(new MenuItem("telehp", "Ping Low Health Allies")).SetValue(false);
                    Menu.AddItem(new MenuItem("teleult", "Ping Dangerous Activity")).SetValue(false);
                }

                root.AddSubMenu(Menu);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.PrintChat("<font color=\"#FFF280\">Exception thrown at CoreSum.CreateMenu: </font>: " + e.Message);
            }

            return this;
        }

        public bool IsReady()
        {
            return Player.GetSpellSlot(Name).IsReady() || 
                ExtraNames.Any(exname => Player.GetSpellSlot(exname).IsReady());
        }

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration
                    || Name == "summonerexhaust") // ignore limit
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Name == "summonerexhaust" ? 0: Duration;
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration
                    || Name == "summonerexhaust") // ignore limit
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot, target);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Name == "summonerexhaust" ? 0 : Duration;
                    }
                }
            }
        }

        public virtual void OnDraw(EventArgs args)
        {

        }

        public virtual void OnTick(EventArgs args)
        {

        }
    }
}
