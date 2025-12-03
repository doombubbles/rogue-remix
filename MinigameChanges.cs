using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.DailyChallenge;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace RogueRemix;

public static class MinigameChanges
{
    [HarmonyPatch(typeof(RogueHexGrid), nameof(RogueHexGrid.CreateTileData), typeof(LegendsTileSaveData))]
    internal static class RogueHexGrid_CreateTileData
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueHexGrid __instance, LegendsTileData __result)
        {
            if (!RogueRemixMod.NoRaces) return;

            var rules = GameData.Instance.rogueData.rogueGameModeRules;

            if (!__result.Is(out RogueTileData rogueTileData) ||
                rogueTileData.tileType != (int) RogueTileType.miniGame) return;

            rogueTileData.minigameData.miniGameType = RogueMiniGameType.leastCash;
            rogueTileData.minigameData.goal =
                rules.GetLeashCashGoal(RogueMapScreen.GetTotalCashForTile(__instance.RogueSaveData, rules));
        }
    }
}