﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
namespace Activator.Summoners
{
    internal class dot : CoreSum
    {
        internal override string Name
        {
            get { return "summonerdot"; }
        }

        internal override string DisplayName
        {
            get { return "Ignite"; }
        }

        internal override string[] ExtraNames
        {
            get { return new[] { "" }; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override int Duration
        {
            get { return 100; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var tar in Activator.Heroes)
            {
                if (!tar.Player.IsValidTarget(600))
                    continue;

                if (tar.Player.IsZombie || tar.Player.HasBuff("summonerdot")) 
                    continue;

                if (!Parent.Item(Parent.Name + "useon" + tar.Player.NetworkId).GetValue<bool>())
                    continue;

                // ignite damagerino
                var ignotedmg = (float) Player.GetSummonerSpellDamage(tar.Player, Damage.SummonerSpell.Ignite);

                // killsteal ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (tar.Player.Health <= ignotedmg)
                        UseSpellOn(tar.Player);
                }

                // combo ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1)
                {
                    var totaldmg = 0d;
                    switch (Player.ChampionName)
                    {
                        case "Ahri":
                            if (!tar.Player.HasBuffOfType(BuffType.Charm) &&
                                Menu.Item("ii" + Player.ChampionName).GetValue<bool>() &&
                                Player.GetSpell(SpellSlot.E).State != SpellState.NotLearned)
                                continue;
                            break;
                        case "Cassiopeia":
                            if (!tar.Player.HasBuffOfType(BuffType.Poison) &&
                                Menu.Item("ii" + Player.ChampionName).GetValue<bool>() &&
                                Player.GetSpell(SpellSlot.E).State != SpellState.NotLearned)
                                continue;

                            totaldmg += Player.GetSpell(SpellSlot.E).State == SpellState.Ready
                                ? Player.GetSpellDamage(tar.Player, SpellSlot.E) * 3
                                : 0;

                            break;
                        case "Diana":
                            if (!tar.Player.HasBuff("dianamoonlight") &&
                                Menu.Item("ii" + Player.ChampionName).GetValue<bool>() &&
                                Player.GetSpell(SpellSlot.Q).State != SpellState.NotLearned)
                                continue;

                            totaldmg += Player.GetSpell(SpellSlot.E).State == SpellState.Ready
                                ? Player.GetSpellDamage(tar.Player, SpellSlot.R)
                                : 0;

                            break;
                    }

                    // aa dmg
                    totaldmg += Orbwalking.InAutoAttackRange(tar.Player)
                        ? Player.GetAutoAttackDamage(tar.Player, true) * 3
                        : 0;

                    // combo damge
                    totaldmg +=
                        Data.Spelldata.DamageLib.Sum(
                            entry =>
                                Player.GetSpell(entry.Value).IsReady(2)
                                    ? entry.Key(Player, tar.Player, Player.GetSpell(entry.Value).Level - 1)
                                    : 0);

                    if (totaldmg + ignotedmg >= tar.Player.Health)
                    {
                        var nearTurret =
                            Activator.Turrets.Find(
                                x => x.Team == tar.Player.Team && tar.Player.Distance(x.ServerPosition) <= 1250);

                        if (nearTurret != null && Menu.Item("itu").GetValue<bool>() &&
                            Player.Level <= Menu.Item("igtu").GetValue<Slider>().Value)
                        {
                            if (Player.CountAlliesInRange(750) == 0 && (totaldmg + ignotedmg / 1.85) < tar.Player.Health)
                                continue;
                        }

                        if (Orbwalking.InAutoAttackRange(tar.Player) && Player.CountAlliesInRange(950) > 0)
                        {
                            if (totaldmg + ignotedmg / 2.5 >= tar.Player.Health)
                                continue;
                        }

                        if (tar.Player.Level <= 4 &&
                            tar.Player.InventoryItems
                                .Any(item =>
                                    item.Id == (ItemId) 2003 ||
                                    item.Id == (ItemId) 2010))
                        {
                            continue;
                        }

                        UseSpellOn(tar.Player, true);
                    }
                }
            }
        }
    }
}
