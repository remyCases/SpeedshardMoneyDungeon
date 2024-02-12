// Copyright (C) 2024 Rémy Cases
// See LICENSE file for extended copyright information.
// This file is part of the Speedshard repository from https://github.com/remyCases/SpeedshardMoneyDungeon.

using ModShardLauncher;
using ModShardLauncher.Mods;
using System.Collections.Generic;
using UndertaleModLib.Models;
using UndertaleModTool;
using System.IO;
using System.Data.Common;

namespace Speedshard_MoneyDungeon;
public class SpeedshardMoneyDungeon : Mod
{
    public override string Author => "zizani";
    public override string Name => "Speedshard - MoneyDungeon";
    public override string Description => "More gold, more rooms, more items, and more.";
    public override string Version => "1.0.0.0";
    public override string TargetVersion => "0.8.2.10";

    public override void PatchMod()
    {
        SpeedshardIni();
        ResetTime();
        DungeonController();
        MoreMoneyContract();
        ArtifactFix();
        MoreGold();
        RareEnchant();
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
        Msl.LoadGML("gml_Object_o_dungeon_controller_Create_0")
            .Apply(DungeonIterator)
            .Save();
    }
    private void MoreMoneyContract()
    {
        Msl.LoadGML("gml_GlobalScript_scr_contract_finish")
            .MatchFrom("var prepaid\nvar money")
            .ReplaceBy(ModFiles, "money_contract1.gml")
            .MatchFromUntil("with (scr_guiCreateContainer", "}")
            .ReplaceBy(ModFiles, "money_contract2.gml")
            .Save();
    }
    private IEnumerable<string> ChestRemoteIterator(IEnumerable<string> enumerable, string table)
    {
        string text = @ModFiles.GetCode("chest_remote.gml");
        string textModif = "";
        bool matchFound = false;
        bool lootFound = false;
        bool untilFound = false;
        foreach (string element in enumerable)
        {
            if (!matchFound && element.Contains('}')) // only once
            {
                matchFound = true;
                yield return element;
                yield return table; // insert table bellow
            }
            else if (matchFound && !lootFound && element.Contains("with"))
            {
                lootFound = true;
                textModif = string.Format(text, element);
            }
            else if (matchFound && lootFound && !untilFound && element.Contains('}')) // last line to be replaced
            {
                untilFound = true;
                yield return textModif;
            }
            else if (untilFound || !matchFound) // after and before replacing
            {
                yield return element;
            }
        }
    }
    private void ArtifactFix()
    {
        Msl.LoadGML("gml_GlobalScript_scr_loot_chestRemoteBastion")
            .Apply(x => ChestRemoteIterator(x, "var artifacts = [3243, 4570, 340, 579, 2609]"))
            .Save();
        
        Msl.LoadGML("gml_GlobalScript_scr_loot_chestRemoteCrypt")
            .Apply(x => ChestRemoteIterator(x, "var artifacts = [6365, 818, 1579, 104, 822, 2609]"))
            .Save();
        
        Msl.LoadGML("gml_GlobalScript_scr_loot_chestRemoteCatacombs")
            .Apply(x => ChestRemoteIterator(x, "var artifacts = [818, 104, 6365, 579, 2609]"))
            .Save();
    }

    private void MoreGold()
    {
        Msl.LoadGML("gml_Object_o_village_standing_Alarm_1")
            .MatchFrom("event")
            .InsertBelow(ModFiles, "more_gold.gml")
            .Save();
    }
    
    private void RareEnchant()
    {
        LocalizationItem localizationItem = new(
            "rare_scroll_enchant",
            "Greater Enchantment Scroll",
            "~lg~Enchants~/~ an item, applying two random bonus.",
            "Ещё один мощный удар Единого Круга по Коллегии зачарователей.;Yet another powerful blow delivered to the Enchanters' Collegium by the United Circle.;这是联合法会给予附魔委员会的又一大打击。;Noch ein erfolgreicher Schlag der Magiergilde gegen die Verzauberergilde.;Otro duro golpe que el Círculo Unido le asestó al Colegio de Encantadores.;Un autre coup puissant porté au Collège des Enchanteurs par le Cercle Uni.;L'ennesimo prova di superiorità inferta dalla Cerchia Unita al Collegio degli Incantatori.;Outro golpe poderoso desferido pela Guilda de Magos à Guilda de Encantadores.;Kolejny potężny cios zadany Kolegium Zaklinaczy przez Zjednoczony Krąg.;Büyücüler Loncası'ndan Efsuncular Loncası'na vurulmuş diğer bir güçlü darbe.;統一学派が附魔術学協会に強い衝撃を与えた巻物。;연합 학회가 변화 마법사 학회에게 또 한 번 강력하게 날린 일격과도 같습니다."
        );
        localizationItem.InjectTable();

        Msl.AddNewEvent(
            "o_inv_scroll_enchant",
            ModFiles.GetCode("inv_scroll_enchant_Other_24.gml"),
            EventType.Other,
            24
        );

        UndertaleGameObject invRareScroll = Msl.AddObject(
            name: "o_inv_rare_scroll_enchant", 
            spriteName: "s_inv_scroll", 
            parentName: "o_inv_scroll_parent", 
            isVisible: true, 
            isPersistent: true, 
            isAwake: true
        );
        invRareScroll.ApplyEvent(ModFiles, 
                new MslEvent("inv_rare_scroll_enchant_Create_0.gml", EventType.Create, 0),
                new MslEvent("inv_rare_scroll_enchant_Other_24.gml", EventType.Other, 24)
        );

        Msl.AddNewEvent(
            "o_skill_enchantment",
            ModFiles.GetCode("skill_enchantment_Create_0.gml"),
            EventType.Create,
            0
        );

        Msl.LoadGML("gml_Object_o_skill_enchantment_Other_11")
            .MatchFrom("scr_rerrol_item_simple")
            .ReplaceBy("scr_rerrol_item_simple(other.quality)")
            .Save();

        UndertaleGameObject lootRareScroll = Msl.AddObject(
            name: "o_loot_rare_scroll_enchant", 
            spriteName: "s_loot_scroll",
            parentName: "o_loot_scroll_parent",
            isVisible: true,
            isPersistent: false,
            isAwake: true,
            collisionShapeFlags: CollisionShapeFlags.Box
        );
        lootRareScroll.ApplyEvent(ModFiles, 
            new MslEvent("loot_rare_scroll_enchant_Create_0.gml", EventType.Create, 0)
        );

        Msl.LoadGML("gml_GlobalScript_scr_loot_chestRemoteCatacombs")
            .MatchFrom("if scr_chance_value(50)\nscr_inventory_add_item")
            .ReplaceBy(ModFiles, "chest_remote_rare_enchant.gml")
            .Save();

        Msl.LoadGML("gml_GlobalScript_scr_loot_chestRemoteCrypt")
            .MatchFrom("if scr_chance_value(75)\nscr_inventory_add_item")
            .ReplaceBy(ModFiles, "chest_remote_rare_enchant.gml")
            .Save();

        Msl.LoadGML("gml_GlobalScript_scr_loot_chestRemoteBastion")
            .MatchFrom("if scr_chance_value(75)\nscr_inventory_add_item")
            .ReplaceBy(ModFiles, "chest_remote_rare_enchant.gml")
            .Save();
    }
}
