using System;
using System.Linq;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.DailyChallenge;
using Il2CppAssets.Scripts.Unity.UI_New.Main.PowersSelect;
namespace RogueRemix;

public static class CampfireChanges
{
    /// <summary>
    /// Campfire sidegrading
    /// </summary>
    [HarmonyPatch(typeof(RogueMentorPanel), nameof(RogueMentorPanel.InventoryIconClicked))]
    internal static class RogueMentorPanel_InventoryIconClicked
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueMentorPanel __instance, InstaTowerDisplay instaTowerDisplay)
        {
            for (var path = 0; path < __instance.upgradeInstaDisplays.Count; path++)
            {
                var upgradeInstaDisplay = __instance.upgradeInstaDisplays[path];
                if (upgradeInstaDisplay == null ||
                    upgradeInstaDisplay.gameObject.active ||
                    instaTowerDisplay.tiers[path] > 0) continue;

                var newTiers = instaTowerDisplay.tiers.ToList();
                var ordered = instaTowerDisplay.tiers.OrderByDescending(i => i).ToArray();
                newTiers[newTiers.IndexOf(ordered[1])] = 0;
                newTiers[path] = ordered[1];

                upgradeInstaDisplay.gameObject.SetActive(true);
                upgradeInstaDisplay.DisplayRogueInsta(instaTowerDisplay.towerBaseId, newTiers.ToArray(),
                    new Action<InstaTowerDisplay>(__instance.InstaUpgradeClicked));
            }
        }
    }
}