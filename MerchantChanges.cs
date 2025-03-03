using System;
using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppNinjaKiwi.Common;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RogueRemix;

public static class MerchantChanges
{
    private static readonly string RerollConfirm = ModContent.Localize<RogueRemixMod>(nameof(RerollConfirm),
        "Are you sure you want to reroll? This will cancel the current trade offer.");

    public static bool IsPopulatingShop { get; private set; }

    public static Vector2Int? lastTile;
    public static int currentRerolls;

    public static void Reroll(RogueMerchantPanel merchantPanel, bool bypassPopup = false)
    {
        if (!bypassPopup &&
            merchantPanel.artifactsMerchantOffersContainer.GetComponentsInChildren<RogueArtifactDisplayIcon>().Any())
        {
            PopupScreen.instance.ShowPopup(PopupScreen.Placement.inGameCenter, "Reroll".Localize(),
                RerollConfirm.Localize(), new Action(() => Reroll(merchantPanel, true)), "OK".Localize(), null,
                "Cancel".Localize(), Popup.TransitionAnim.Scale);
            return;
        }

        for (var i = 0; i < RerollCost; i++)
        {
            merchantPanel.RogueSaveData.RemoveArtifactFromInventory("Token");
        }

        merchantPanel.merchantTile.seed++;
        currentRerolls++;
        merchantPanel.Open(merchantPanel.merchantTile);

    }

    public static int RerollCost => (currentRerolls + 1) * 3;

    /// <summary>
    /// Add reroll button
    /// </summary>
    [HarmonyPatch(typeof(RogueMerchantPanel), nameof(RogueMerchantPanel.Open))]
    internal static class RogueMerchantPanel_Open
    {
        [HarmonyPrefix]
        internal static void Prefix(RogueMerchantPanel __instance, RogueTile merchantTile)
        {
            if (__instance.tradeBtn.transform.parent.FindChildWithName("Reroll") == null &&
                merchantTile.coords == lastTile)
            {
                merchantTile.seed += currentRerolls;
            }

            if (lastTile != merchantTile.coords)
            {
                currentRerolls = 0;
                lastTile = merchantTile.coords;
            }

            foreach (var selectedInventoryArtifactIcon in __instance.selectedInventoryArtifactIcons)
            {
                __instance.RemoveInventoryIconFromOffer(selectedInventoryArtifactIcon);
            }
            foreach (var selectedMerchantArtifactIcon in __instance.selectedMerchantArtifactIcons)
            {
                __instance.RemoveMerchantIconFromOffer(selectedMerchantArtifactIcon);
            }
        }

        [HarmonyPostfix]
        internal static void Postfix(RogueMerchantPanel __instance)
        {
            if (__instance.tradeBtn.transform.parent.FindChildWithName("Reroll") == null)
            {
                var reroll = Object.Instantiate(__instance.tradeBtn.gameObject, __instance.tradeBtn.transform.parent);
                reroll.name = "Reroll";

                var tokenDisplay = Object.Instantiate(__instance.artifactDisplayPrefab, reroll.transform);
                tokenDisplay.name = "TokenDisplay";
                tokenDisplay.SetActive(true);
                tokenDisplay.RemoveComponent<Button>();

                var rectTransform = tokenDisplay.GetComponent<RectTransform>();
                rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                var displayIcon = tokenDisplay.GetComponent<RogueArtifactDisplayIcon>();
                displayIcon.Bind(GameData.Instance.artifactsData.GetBaseArtifactModel("Token"), null);

                var button = reroll.GetComponent<Button>();
                button.SetOnClick(() => Reroll(__instance));
                button.interactable = true;

                reroll.GetComponent<Image>().SetSprite(VanillaSprites.BlueBtnLong);
                reroll.GetComponentInChildren<NK_TextMeshProUGUI>().OverrideText("Reroll");

                // var description = __instance.transform.Find("Panel/Top/Panel/Description").GetComponent<NK_TextMeshProUGUI>();
                // description.localizeKey = description.GetText().Split("\n").Last();

                __instance.RefreshUIs();
            }
        }
    }

    /// <summary>
    /// Update reroll cost, intractability
    /// </summary>
    [HarmonyPatch(typeof(RogueMerchantPanel), nameof(RogueMerchantPanel.RefreshUIs))]
    internal static class RogueMerchantPanel_RefreshUIs
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueMerchantPanel __instance)
        {
            if (__instance.GetComponentFromChildrenByName<Button>("Reroll").Is(out var button))
            {
                button.interactable = __instance.RogueSaveData.TokenCount() >= RerollCost;

                if (button.GetComponentFromChildrenByName<RogueArtifactDisplayIcon>("TokenDisplay")
                    .Is(out var tokenDisplay))
                {
                    tokenDisplay.StackCount = RerollCost;
                }
            }

            var hasBoostsInTrade = __instance.artifactsMerchantOffersContainer
                .GetComponentsInChildren<RogueArtifactDisplayIcon>().Any(icon => icon.artifactModel.IsBoost);
            foreach (var activeMerchantIcon in __instance.activeMerchantIcons.Where(icon => icon.artifactModel.IsBoost))
            {
                activeMerchantIcon.ToggleAddButtonInteractable(!hasBoostsInTrade);
            }
        }
    }

    /// <summary>
    /// Track when randomly generated artifacts are coming from the shop
    /// </summary>
    [HarmonyPatch(typeof(RogueMerchantPanel), nameof(RogueMerchantPanel.PopulateMerchantArtifacts))]
    internal static class RogueMerchantPanel_PopulateMerchantArtifacts
    {
        [HarmonyPrefix]
        internal static void Prefix(RogueMerchantPanel __instance)
        {
            IsPopulatingShop = true;
        }

        [HarmonyPostfix]
        internal static void Postfix(RogueMerchantPanel __instance)
        {
            IsPopulatingShop = false;
        }
    }

    /*
    /// <summary>
    /// Allow multiple shop trades
    /// </summary>
    [HarmonyPatch(typeof(RogueMerchantPanel), nameof(RogueMerchantPanel.ShowLootPopups))]
    internal static class RogueMerchantPanel_ShowLootPopups
    {
        [HarmonyPrefix]
        internal static bool Prefix(RogueMerchantPanel __instance)
        {
            if (__instance.lootToShow.Count <= 0)
            {
                __instance.Open(__instance.merchantTile);
                return false;
            }

            return true;
        }
    }
    */
}