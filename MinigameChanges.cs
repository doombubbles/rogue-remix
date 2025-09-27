using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace RogueRemix;

public static class MinigameChanges
{
    [HarmonyPatch(typeof(RogueMap), nameof(RogueMap.CreateTileDatas))]
    internal static class RogueMap_CreateTileDatas
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueMap __instance, Dictionary<Vector2Int, RogueTileData> __result)
        {
            if (!RogueRemixMod.NoRaces) return;

            var rules = GameData.Instance.rogueData.rogueGameModeRules;

            foreach (var tileData in __result.Values())
            {
                if (tileData.tileType != RogueTileType.miniGame) continue;

                tileData.minigameData.miniGameType = RogueMiniGameType.leastCash;
                tileData.minigameData.goal = rules.GetLeashCashGoal(__instance.rogueMapScreen.GetTotalCashForTile());
            }
        }
    }
}