using System.Linq;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Unity.Analytics;
using Il2CppAssets.Scripts.Unity.UI_New.DailyChallenge;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using UnityEngine;
namespace RogueRemix;

public static class SkippingChanges
{
    private static bool blockNextLifeLossAnalytics;
    private static bool blockNextLifeLossAnim;

#if DEBUG
    private static readonly bool DebugFreeSkipping = true;
#else
    private static readonly bool DebugFreeSkipping = false;
#endif

    [HarmonyPatch(typeof(RogueTileInfoPanel), nameof(RogueTileInfoPanel.Bind))]
    internal static class RogueTileInfoPanel_Bind
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueTileInfoPanel __instance, RogueTile selectedTile, bool showContinueBtn,
            bool showSkipBtn)
        {
            if (!RogueRemixMod.SkippingRemix && !DebugFreeSkipping) return;

            __instance.skipBtn
                .GetComponentsInChildren<RectTransform>(true)
                .First(transform => transform.name == "Cost")
                .gameObject.SetActive(!DebugFreeSkipping &&
                                      (showContinueBtn ||
                                       selectedTile.tileType != RogueTileType.pathStandardGame ||
                                       !showSkipBtn));
        }
    }

    [HarmonyPatch(typeof(RogueTileInfoPanel), nameof(RogueTileInfoPanel.SkipTileClicked))]
    internal static class RogueTileInfoPanel_SkipTileClicked
    {
        [HarmonyPrefix]
        internal static void Prefix(RogueTileInfoPanel __instance)
        {
            if (!__instance.skipBtn
                    .GetComponentsInChildren<RectTransform>(true)
                    .First(transform => transform.name == "Cost")
                    .gameObject.active)
            {
                var rogueSaveData = RogueLegendsManager.instance.RogueSaveData;
                rogueSaveData.lives++;
                rogueSaveData.unseenLivesLost--;
                rogueSaveData.currentStageStats.heartsLost--;
                rogueSaveData.totalCampaignStats.heartsLost--;

                blockNextLifeLossAnim = true;
                blockNextLifeLossAnalytics = true;
            }
        }
    }

    [HarmonyPatch(typeof(AnalyticsManager), nameof(AnalyticsManager.LegendsLoseHearts))]
    internal static class AnalyticsManager_LegendsLoseHearts
    {
        [HarmonyPrefix]
        internal static bool Prefix()
        {
            if (blockNextLifeLossAnalytics)
            {
                blockNextLifeLossAnalytics = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RogueMapScreen), nameof(RogueMapScreen.LivesRemovedAnim))]
    internal static class RogueMapScreen_LivesRemovedAnim
    {
        [HarmonyPrefix]
        internal static bool Prefix()
        {
            if (blockNextLifeLossAnim)
            {
                blockNextLifeLossAnim = false;
                return false;
            }
            return true;
        }
    }
}