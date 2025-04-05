using System;
using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppInterop.Runtime;
using Il2CppNinjaKiwi.Common;
using RogueRemix.NewArtifacts;
using UnityEngine;
using UnityEngine.UI;

namespace RogueRemix;

public static class ArtifactSelling
{
    private const int ArtifactsPerRow = 8;

    private static readonly string SellArtifact =
        ModContent.Localize<RogueRemixMod>(nameof(SellArtifact), "Sell Artifact");
    private static readonly string SellArtifactDescription =
        ModContent.Localize<RogueRemixMod>(nameof(SellArtifactDescription),
            "This Artifact will be removed from your inventory, and you will gain its value in Tokens. Are you sure you want to sell it?");


    public static int TokenCount(this RogueGameSaveData saveData) =>
        saveData.artifactsInventory.Where(loot => loot.baseId == "Token").Count;

    private static void AddSpacer(Transform parent, int index)
    {
        var spacer = new GameObject("Spacer", Il2CppType.Of<RectTransform>());
        var transform = spacer.GetComponent<RectTransform>();
        transform.SetParent(parent);
        transform.sizeDelta = parent.GetComponent<GridLayoutGroup>().cellSize;
        transform.SetSiblingIndex(index);
    }

    /// <summary>
    /// Reset artifacts list
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.PopulateArtifacts))]
    internal static class DisplayArtifactsPanel_PopulateArtifacts
    {
        [HarmonyPrefix]
        internal static void Prefix(DisplayArtifactsPanel __instance)
        {
            __instance.artifactsInventoryContainer.transform.DestroyAllChildren();
            __instance.activeArtifactIcons.Clear();
        }
    }

    /// <summary>
    /// Reogranize artifacts list
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.OpenShowInventory))]
    internal static class DisplayArtifactsPanel_OpenShowInventory
    {
        [HarmonyPostfix]
        internal static void Postfix(DisplayArtifactsPanel __instance)
        {
            var total = __instance.activeArtifactIcons.Count;
            var token = __instance.activeArtifactIcons.FirstOrDefault(icon => icon.artifactModel.baseId == "Token");
            var nonBoosts = __instance.activeArtifactIcons.Where(icon => !icon.artifactModel.IsBoost);
            var boosts = __instance.activeArtifactIcons.Where(icon => icon.artifactModel.IsBoost);

            if (token != null && token.isActiveAndEnabled)
            {
                token.transform.SetSiblingIndex(0);
            }
            foreach (var boost in boosts)
            {
                boost.transform.SetSiblingIndex(total - 1);
            }

            if (token != null && token.isActiveAndEnabled)
            {
                token.transform.SetSiblingIndex(nonBoosts.Count - 1);
                while (token.transform.GetSiblingIndex() % ArtifactsPerRow > 0)
                {
                    AddSpacer(__instance.artifactsInventoryContainer.transform, nonBoosts.Count - 1);
                }
            }
            /*else if (boosts.Count > 0)
            {
                var startIndex = nonBoosts.ToArray().Max(icon => icon.transform.GetSiblingIndex()) + 1;
                while (boosts.First().transform.GetSiblingIndex() % ArtifactsPerRow > 0)
                {
                    AddSpacer(__instance.artifactsInventoryContainer.transform, startIndex);
                }
            }*/

        }
    }

    /// <summary>
    /// Switch ok button when showing sell panel
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.DiscardClicked))]
    internal static class DisplayArtifactsPanel_DiscardClicked
    {
        [HarmonyPrefix]
        internal static bool Prefix(DisplayArtifactsPanel __instance)
        {
            if (!RogueRemixMod.SellingReplacesDiscarding) return true;

            PopupScreen.instance.ShowPopup(PopupScreen.Placement.inGameCenter, SellArtifact.Localize(),
                SellArtifactDescription.Localize(), new Action(__instance.DiscardArtifact), "OK".Localize(), null,
                "Cancel".Localize(), Popup.TransitionAnim.Scale);

            return false;
        }
    }

    /// <summary>
    /// Handle selling
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.DiscardArtifact))]
    internal static class DisplayArtifactsPanel_DiscardArtifact
    {
        [HarmonyPrefix]
        internal static bool Prefix(DisplayArtifactsPanel __instance)
        {
            if (!RogueRemixMod.SellingReplacesDiscarding) return true;

            if (__instance.selectedIcon == null || !__instance.selectedIcon.gameObject.active) return false;

            var artifactModel = __instance.selectedIcon.artifactModel;

            if (artifactModel.baseId == "Token") return true;

            if (!Upcycling.Handle(artifactModel))
            {
                TokenChanges.RewardTokens(artifactModel.ArtifactPower);
            }

            return true;
        }
    }
}