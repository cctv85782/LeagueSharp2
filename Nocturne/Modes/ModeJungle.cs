﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Nocturne.Common;
using Color = SharpDX.Color;

namespace Nocturne.Modes
{
    internal static class ModeJungle
    {
        public static Menu LocalMenu { get; private set; }

        public static void Initialize(Menu mainMenu)
        {
            LocalMenu = new Menu("Jungle", "Jungle");
            {
                /*
                LocalMenu.AddItem(new MenuItem("Jungle.Mode", "Jungle Clear Mode: ").SetValue(new StringList(new[] { "Simple Mode", "Advanced Mode" }, 0)).SetFontStyle(FontStyle.Bold, Color.Aqua)).ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs args)
                    {
                        InitializeRefreshMenuItems();
                    };
                */
                LocalMenu.AddItem(new MenuItem("Jungle.Enable", ":: Quick Enable/Disable Jungle Farm ").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle, true))).Permashow(true, ObjectManager.Player.ChampionName + " : " + "Farm Jungle", Colors.ColorPermaShow);

                //InitializeSimpleMenu();
                InitializeAdvancedMenu();

                LocalMenu.AddItem(new MenuItem("Jungle.Youmuu.BaronDragon", "Items: Use for Baron/Dragon").SetValue(new StringList(new[] {"Off", "Dragon", "Baron", "Both"}, 3))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
                LocalMenu.AddItem(new MenuItem("Jungle.Youmuu.BlueRed", "Items: Use for Blue/Red").SetValue(new StringList(new[] { "Off", "Red", "Blue", "Both" }, 3))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
                LocalMenu.AddItem(new MenuItem("Jungle.Item", "Items: Other (Like Tiamat/Hydra)").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);

            }
            mainMenu.AddSubMenu(LocalMenu);
            //InitializeRefreshMenuItems();
            Game.OnUpdate += OnUpdate;
        }

        static void InitializeSimpleMenu()
        {
            string[] strQSimple = new string[5];
            {
                strQSimple[0] = "Off";
                strQSimple[1] = "Just Big Mobs";
                for (var i = 2; i < 5; i++)
                {
                    strQSimple[i] = "Minion Count >= " + i;
                }
                LocalMenu.AddItem(new MenuItem("Jungle.Simple.UseQ", "Q:").SetValue(new StringList(strQSimple, 0))).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor()).SetTag(1);
            }

            LocalMenu.AddItem(new MenuItem("Jungle.Simple.UseE", "E:").SetValue(new StringList(new[] { "Off", "On", "Just Big Mobs" }, 2)).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor())).SetTag(1);
        }

        static void InitializeAdvancedMenu()
        {
            LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseQ.Big1", "Q: [Blue/Red]").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor())).SetTag(2);
            LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseQ.Big2", "Q: [Baron/Dragon]").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor())).SetTag(2);
            LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseQ.Big3", "Q: [Other Big Mobs]").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor())).SetTag(2);

            string[] strQ = new string[5];
            {
                strQ[0] = "Off";
                for (var i = 1; i < 5; i++)
                {
                    strQ[i] = "Minion Count >= " + i;
                }
                LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseQ.Small", "Q: [Small Mobs]").SetValue(new StringList(strQ, 0))).SetFontStyle(FontStyle.Regular, PlayerSpells.Q.MenuColor()).SetTag(2);
            }


            LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseE.Big1", "E: [Blue/Red]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, PlayerSpells.E.MenuColor()).SetTag(2);
            LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseE.Big2", "E: [Baron/Dragon]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, PlayerSpells.E.MenuColor()).SetTag(2);
            LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseE.Big3", "E: [Other Big Mobs]").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, PlayerSpells.E.MenuColor()).SetTag(2);
            LocalMenu.AddItem(new MenuItem("Jungle.Advanced.UseE.Big4", "E: [Small Mobs]").SetValue(new StringList(new[] { "Off", "On" }, 0))).SetFontStyle(FontStyle.Regular, PlayerSpells.E.MenuColor()).SetTag(2);
        }

        private static void InitializeRefreshMenuItems()
        {
            int argsValue = LocalMenu.Item("Jungle.Mode").GetValue<StringList>().SelectedIndex;

            foreach (var item in LocalMenu.Items)
            {
                item.Show(true);
                switch (argsValue)
                {
                    case 0:
                        if (item.Tag == 1)
                        {
                            item.Show(false);
                        }
                        break;
                    case 1:
                        if (item.Tag == 2)
                        {
                            item.Show(false);
                        }
                        break;
                }
            }
        }


        private static void OnUpdate(EventArgs args)
        {
            if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && LocalMenu.Item("Jungle.Enable").GetValue<KeyBind>().Active)
            {
                ExecuteAdvancedMode();
            }
        }

        private static void ExecuteSimpleMode()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, PlayerSpells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count <= 0)
            {
                return;
            }

        }
        private static void ExecuteAdvancedMode()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, PlayerSpells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0)
            {
                return;
            }

            var mob = mobs[0];

            if (ObjectManager.Player.ManaPercent < CommonManaManager.JungleMinManaPercent(mob))
            {
                return;
            }
            
            if (PlayerSpells.Q.IsReady() && mob.IsValidTarget(PlayerSpells.Q.Range))
            {
                if (LocalMenu.Item("Jungle.Advanced.UseQ.Big1").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Blue || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Red)
                    {
                        if (PlayerSpells.Q.IsReady())
                        {
                            PlayerSpells.Q.Cast(mob);
                        }
                    }
                }

                if (LocalMenu.Item("Jungle.Advanced.UseQ.Big2").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Baron || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Dragon)
                    {
                        if (PlayerSpells.Q.IsReady())
                        {
                            PlayerSpells.Q.Cast(mob);
                        }
                    }
                }

                if (mob.Health > ObjectManager.Player.TotalAttackDamage*2)
                {
                    if (LocalMenu.Item("Jungle.Advanced.UseQ.Big3").GetValue<StringList>().SelectedIndex != 0)
                    {
                        if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big &&
                            (
                                CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Dragon || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Baron
                                || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Red || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Blue)
                            )
                        {
                            if (PlayerSpells.Q.IsReady())
                            {
                                PlayerSpells.Q.Cast(mob);
                            }
                        }
                    }

                    if (LocalMenu.Item("Jungle.Advanced.UseQ.Small").GetValue<StringList>().SelectedIndex != 0)
                    {
                        if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) != CommonManaManager.MobTypes.Big)
                        {
                            if (mobs.Count >= LocalMenu.Item("Jungle.Advanced.UseQ.Small").GetValue<StringList>().SelectedIndex)
                            {
                                if (PlayerSpells.Q.IsReady())
                                {
                                    PlayerSpells.Q.Cast(mob);
                                }
                            }
                        }
                    }
                }
            }

            if (PlayerSpells.E.IsReady() && mob.IsValidTarget(PlayerSpells.E.Range) && mob.Health > ObjectManager.Player.TotalAttackDamage * 3)
            {
                if (LocalMenu.Item("Jungle.Advanced.UseE.Big1").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Blue || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Red)
                    {
                        if (PlayerSpells.E.IsReady())
                        {
                            PlayerSpells.E.Cast(mob);
                        }
                    }
                }

                if (LocalMenu.Item("Jungle.Advanced.UseE.Big2").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Baron || CommonManaManager.GetMobType(mob) == CommonManaManager.MobTypes.Dragon)
                    {
                        if (PlayerSpells.E.IsReady())
                        {
                            PlayerSpells.E.Cast(mob);
                        }
                    }
                }

                if (LocalMenu.Item("Jungle.Advanced.UseE.Big3").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) == CommonManaManager.MobTypes.Big &&
                        (
                            CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Dragon || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Baron
                            || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Red || CommonManaManager.GetMobType(mob) != CommonManaManager.MobTypes.Blue)
                        )
                    {
                        if (PlayerSpells.E.IsReady())
                        {
                            PlayerSpells.E.Cast(mob);
                        }
                    }
                }

                if (LocalMenu.Item("Jungle.Advanced.UseE.Big4").GetValue<StringList>().SelectedIndex != 0)
                {
                    if (CommonManaManager.GetMobType(mob, CommonManaManager.FromMobClass.ByType) != CommonManaManager.MobTypes.Big)
                    {
                        if (PlayerSpells.E.IsReady())
                        {
                            PlayerSpells.E.Cast(mob);
                        }
                    }
                }
            }
        }
    }
}
