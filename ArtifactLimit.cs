using System;
using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Simulation.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppInterop.Runtime;
using Il2CppNinjaKiwi.Common;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using RogueRemix.NewArtifacts;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RogueRemix;

public static class ArtifactLimit
{
    private static bool showingArtifactSellPanel;

    private static readonly string SellArtifact =
        ModContent.Localize<RogueRemixMod>(nameof(SellArtifact), "Sell Artifact");
    private static readonly string SellArtifactDescription =
        ModContent.Localize<RogueRemixMod>(nameof(SellArtifactDescription),
            "This Artifact will be removed from your inventory, and you will gain its value in Tokens. Are you sure you want to sell it?");
    private static readonly string ArtifactLimitExceeded =
        ModContent.Localize<RogueRemixMod>(nameof(ArtifactLimitExceeded), "Artifact Limit Exceeded");
    private static readonly string SellAnArtifact =
        ModContent.Localize<RogueRemixMod>(nameof(SellAnArtifact), "You must select an Artifact to sell");

    private const int ArtifactsPerRow = 8;

    public static int ArtifactCount(this RogueGameSaveData saveData) =>
        saveData.artifactsInventory.Where(loot => loot.lootType == RogueLootType.permanent &&
                                                  loot.baseId != "Token" &&
                                                  !loot.artifactName.Contains("BoostArtifact")).Count;

    public static int BoostCount(this RogueGameSaveData saveData) =>
        saveData.artifactsInventory.Where(loot => loot.lootType == RogueLootType.permanent &&
                                                  loot.baseId != "Token" &&
                                                  loot.artifactName.Contains("BoostArtifact")).Count;


    public static int TokenCount(this RogueGameSaveData saveData) =>
        saveData.artifactsInventory.Where(loot => loot.baseId == "Token").Count;

    public static int MaxArtifacts(this RogueGameSaveData saveData) =>
        RogueRemixMod.BaseArtifactLimit +
        saveData.artifactsInventory
            .ToArray()
            .Sum(artifact => artifact.IsArtifact<ArtifactExpansion>() ? artifact.tier : 0);

    private static void AddSpacer(Transform parent, int index)
    {
        var spacer = new GameObject("Spacer", Il2CppType.Of<RectTransform>());
        var transform = spacer.GetComponent<RectTransform>();
        transform.SetParent(parent);
        transform.sizeDelta = parent.GetComponent<GridLayoutGroup>().cellSize;
        transform.SetSiblingIndex(index);
    }

    /// <summary>
    /// Only add insta monkeys from artifacts if they stay in your loadout
    /// </summary>
    [HarmonyPatch(typeof(LegendsManager), nameof(LegendsManager.OnArtifactAdded))]
    internal static class LegendsManager_OnArtifactAdded
    {
        [HarmonyPrefix]
        internal static void Prefix(LegendsManager __instance, ArtifactLoot artifact, ref bool ignoreItemArtifact)
        {
            if (!RogueRemixMod.ArtifactLimit) return;

            if (artifact.Is(out ItemArtifact itemArtifact) && !ignoreItemArtifact)
            {
                ignoreItemArtifact = true;
                var tower = itemArtifact.itemArtifactModel.GetTower();
                if (tower != null)
                {
                    TaskScheduler.ScheduleTask(() =>
                    {
                        tower.uniqueId = __instance.GetNextInstaUniqueId();
                        __instance.AddInstaToInventory(tower);
                    }, () => __instance.RogueSaveData.ArtifactCount() <= __instance.RogueSaveData.MaxArtifacts());
                }
            }
        }
    }

    /// <summary>
    /// Check for being above artifact limit
    /// </summary>
    [HarmonyPatch(typeof(RogueMap), nameof(RogueMap.Update))]
    internal static class RogueMap_Update
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueMap __instance)
        {
            if (!RogueRemixMod.ArtifactLimit || __instance.RogueSaveData is not {isInitialized: true}) return;

            if (__instance.RogueSaveData.ArtifactCount() > __instance.RogueSaveData.MaxArtifacts())
            {
                var displayArtifactsPanel = __instance.rogueMapScreen.artifactDisplay;
                if (!displayArtifactsPanel.isActiveAndEnabled &&
                    !__instance.rogueMapScreen.merchantPanel.isActiveAndEnabled &&
                    !__instance.rogueMapScreen.extractionPanel.isActiveAndEnabled &&
                    !PopupScreen.instance.IsPopupActive())
                {
                    showingArtifactSellPanel = true;
                    displayArtifactsPanel.OpenShowInventory();
                }
            }
            else if (showingArtifactSellPanel)
            {
                showingArtifactSellPanel = false;
                Object.FindObjectOfType<DisplayArtifactsPanel>(true).Close();
            }
        }
    }

    /// <summary>
    /// Setup UI for force artifact sell panel
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.ResetUIs))]
    internal static class DisplayArtifactsPanel_ResetUIs
    {
        [HarmonyPostfix]
        internal static void Postfix(DisplayArtifactsPanel __instance)
        {
            if (!RogueRemixMod.ArtifactLimit) return;

            __instance.bgClose.interactable = !showingArtifactSellPanel;
            __instance.discardBtn.gameObject.SetActive(!showingArtifactSellPanel);

            __instance.closeBtn.transform.GetComponentInChildren<NK_TextMeshProUGUI>()
                .OverrideText(showingArtifactSellPanel ? "Sell" : "OK");

            __instance.closeBtn.image.LoadSprite(showingArtifactSellPanel
                ? new SpriteReference(VanillaSprites.RedBtnLong)
                : new SpriteReference(VanillaSprites.GreenBtnLong));
            __instance.closeBtn.onClick.RemoveAllListeners();
            __instance.closeBtn.onClick.AddListener(showingArtifactSellPanel
                ? __instance.DiscardClicked
                : __instance.CloseClicked);

            if (showingArtifactSellPanel)
            {
                __instance.artifactNameTxt.SetText(ArtifactLimitExceeded.Localize());
                __instance.artifactDescTxt.SetText(SellAnArtifact.Localize());
            }

            __instance.transform.FindChildWithName("Banner").GetComponentFromChildrenByName<NK_TextMeshProUGUI>("Text")
                .OverrideText("Artifacts");
        }
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
            if (!RogueRemixMod.ArtifactLimit) return;

            __instance.artifactsInventoryContainer.transform.DestroyAllChildren();
            __instance.activeArtifactIcons.Clear();
        }
    }

    /// <summary>
    /// Reogranize artifacts list, display artifact limit
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.OpenShowInventory))]
    internal static class DisplayArtifactsPanel_OpenShowInventory
    {
        [HarmonyPostfix]
        internal static void Postfix(DisplayArtifactsPanel __instance)
        {
            if (!RogueRemixMod.ArtifactLimit) return;

            __instance.transform.FindChildWithName("Banner")
                .GetComponentFromChildrenByName<NK_TextMeshProUGUI>("Text")
                .OverrideText("Artifacts".Localize() +
                              $" ({__instance.RogueSaveData.ArtifactCount()}/{__instance.RogueSaveData.MaxArtifacts()})");

            if (showingArtifactSellPanel)
            {
                __instance.discardBtn.gameObject.SetActive(false);

                foreach (var artifactIcon in __instance.activeArtifactIcons.ToArray())
                {
                    if (artifactIcon.artifactModel.baseId == "Token" || artifactIcon.artifactModel.IsBoost)
                    {
                        artifactIcon.gameObject.Destroy();
                        __instance.activeArtifactIcons.Remove(artifactIcon);
                    }
                }
            }
            else
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
            if (!RogueRemixMod.ArtifactLimit) return true;

            if (showingArtifactSellPanel)
            {
                __instance.DiscardArtifact();
            }
            else
            {
                PopupScreen.instance.ShowPopup(PopupScreen.Placement.inGameCenter, SellArtifact.Localize(),
                    SellArtifactDescription.Localize(), new Action(__instance.DiscardArtifact), "OK".Localize(), null,
                    "Cancel".Localize(), Popup.TransitionAnim.Scale);
            }

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
            if (!RogueRemixMod.ArtifactLimit) return true;

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

    /// <summary>
    /// Block closing if above artifact
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.Close))]
    internal static class DisplayArtifactsPanel_Close
    {
        [HarmonyPrefix]
        internal static bool Prefix(DisplayArtifactsPanel __instance)
        {
            return !RogueRemixMod.ArtifactLimit || !showingArtifactSellPanel;
        }
    }
}