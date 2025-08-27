using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;

namespace RogueRemix;

public static class MinigameChanges
{
    [HarmonyPatch(typeof(RogueMap), nameof(RogueMap.CreateTile))]
    internal static class RogueMap_CreateTile
    {
        [HarmonyPrefix]
        internal static void Prefix(RogueMap __instance, RogueTileData tileData)
        {
            if (!RogueRemixMod.NoRaces || tileData.tileType != RogueTileType.miniGame) return;

            var rules = GameData.Instance.rogueData.rogueGameModeRules;

            tileData.minigameData.miniGameType = RogueMiniGameType.leastCash;
            tileData.minigameData.goal = rules.GetLeashCashGoal(__instance.rogueMapScreen.GetTotalCashForTile());
        }
    }

}