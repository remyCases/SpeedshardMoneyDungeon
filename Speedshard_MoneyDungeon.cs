// Copyright (C) 2024 Rémy Cases
// See LICENSE file for extended copyright information.
// This file is part of the Speedshard repository from https://github.com/remyCases/SpeedshardMoneyDungeon.

using ModShardLauncher;
using ModShardLauncher.Mods;
using System.Collections.Generic;

namespace Speedshard_MoneyDungeon;
public class SpeedshardMoneyDungeon : Mod
{
    public override string Author => "zizani";
    public override string Name => "Speedshard - MoneyDungeon";
    public override string Description => "mod_description";
    public override string Version => "0.5.0.0";
    public override string TargetVersion => "0.8.2.10";

    public override void PatchMod()
    {
        SpeedshardIni();
        ResetTime();
        DungeonController();
        MoreMoneyContract();
    }
    private void SpeedshardIni()
    {
        Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit")
            .MatchFrom("scr_sessionDataInit\n{")
            .InsertBelow(ModFiles, "load_ini.gml")
            .Save();
    }
    static private void ResetTime()
    {
        Msl.LoadGML("gml_GlobalScript_scr_dungeon_reset_time")
            .MatchFrom("scr_globaltile_dungeon_set")
            .ReplaceBy("scr_globaltile_dungeon_set(\"dungeon_reset\", (global.dungeon_reset_modifier * (1 + (_wilderness / 100))), argument0, argument1)")
            .Save();
    }
    static private IEnumerable<string> DungeonIterator(IEnumerable<string> enumerable)
    {
        foreach (string element in enumerable)
        {
            if (element.Contains("ROOMSECRET"))
            {
                yield return "ROOMSECRET = scr_chance_value(global.secret_room_chance)";
            }
            else if (element.Contains("ROOMNUMBER"))
            {
                yield return "ROOMNUMBER = irandom_range(global.number_room_min, global.number_room_max)";
            }
            else if (element.Contains("MAXCHEST"))
            {
                yield return "MAXCHEST = irandom_range(global.number_chest_min, global.number_chest_max)";
            } 
            else
            {
                yield return element;
            }
        }
    }
    static private void DungeonController()
    {
        FileEnumerable<string> controller = Msl.LoadGML("gml_Object_o_dungeon_controller_Create_0");
        FileEnumerable<string> newController = new(controller.header, DungeonIterator(controller.ienumerable));
        newController.Save();
    }
    private void MoreMoneyContract()
    {
        Msl.LoadGML("gml_GlobalScript_scr_contract_finish")
            .MatchFrom("var prepaid\nvar money")
            .ReplaceBy(ModFiles, "money_contract1")
            .MatchFromUntil("with (scr_guiCreateContainer", "}")
            .ReplaceBy(ModFiles, "money_contract2")
            .Save();
    }
}
