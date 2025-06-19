using System.Linq;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppNinjaKiwi.Common;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace RogueRemix;

public static class ArtifactUpgrading
{
    public static bool PopulatingOwned { get; private set; }

    public static void HandleUpgradeIcon(RogueArtifactDisplayIcon icon, string artifactName)
    {
        if (!RogueRemixMod.ArtifactUpgrading ||
            RogueLegendsManager.instance?.RogueSaveData == null ||
            artifactName.Contains("BoostArtifact") ||
            artifactName.StartsWith("Token")) return;

        var upgradeIcon = icon.GetComponentsInChildren<ModHelperImage>(true)
            .FirstOrDefault(image => image.name == "Upgrade");

        if (upgradeIcon == null)
        {
            upgradeIcon = icon.gameObject.AddModHelperComponent(ModHelperImage.Create(new Info("Upgrade")
            {
                Size = 110,
                Pivot = new Vector2(0, 1),
                Anchor = new Vector2(0, 1)
            }, VanillaSprites.UpgradeBtn));
        }

        upgradeIcon.SetActive(!PopulatingOwned &&
                              artifactName != "Token" &&
                              RogueLegendsManager.instance.RogueSaveData.artifactsInventory.Any(loot =>
                                  loot.artifactName == artifactName));
    }

    /// <summary>
    /// Allow duplicates of the same tier to appear
    /// </summary>
    [HarmonyPatch(typeof(RogueLegendsManager), nameof(RogueLegendsManager.CheckIfArtifactOwned))]
    internal static class RogueLegendsManager_CheckIfArtifactOwned
    {
        [HarmonyPrefix]
        internal static void Prefix(ref int tier)
        {
            if (!RogueRemixMod.ArtifactUpgrading) return;

            tier++;
            if (tier > 2) tier = 2;
        }
    }

    /// <summary>
    /// Handle upgrading when you recieve a duplicate
    /// </summary>
    [HarmonyPatch(typeof(RogueGameSaveData), nameof(RogueGameSaveData.AddArtifactToInventory))]
    internal static class RogueGameSaveData_AddArtifactToInventory
    {
        [HarmonyPrefix]
        internal static void Prefix(RogueGameSaveData __instance, ArtifactLoot artifact, ref bool allowStacking)
        {
            if (!RogueRemixMod.ArtifactUpgrading) return;

            if (__instance.artifactsInventory.Any(loot => loot.artifactName == artifact.artifactName) &&
                GameData.Instance.artifactsData.artifactDatas.Values()
                    .Select(data => data.ArtifactModel().Cast<ArtifactModelBase>())
                    .FirstOrDefault(m => !m.IsBoost && m.baseId == artifact.baseId && m.tier == artifact.tier + 1)
                    .Is(out var artifactModel))
            {
                __instance.RemoveArtifactFromInventory(artifact.artifactName);
                artifact.tier++;
                artifact.artifactName = artifactModel!.name;
            }

            if (artifact.artifactName.Contains("BoostArtifact") || artifact.artifactName.StartsWith("Token"))
            {
                allowStacking = true;
            }
        }
    }

    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.PopulateArtifacts))]
    internal static class DisplayArtifactsPanel_PopulateArtifacts
    {
        [HarmonyPrefix]
        internal static void Prefix(DisplayArtifactsPanel __instance)
        {
            PopulatingOwned = true;
        }

        [HarmonyPostfix]
        internal static void Postfix(DisplayArtifactsPanel __instance)
        {
            PopulatingOwned = false;
        }
    }

    /// <summary>
    /// Show upgrade icon
    /// </summary>
    [HarmonyPatch(typeof(RogueArtifactDisplayIcon), nameof(RogueArtifactDisplayIcon.Bind))]
    internal static class RogueArtifactDisplayIcon_Bind
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueArtifactDisplayIcon __instance, ArtifactModelBase artifactModel,
            RogueArtifactDisplayIcon.ArtifactSelected? onAddCallback,
            RogueArtifactDisplayIcon.ArtifactSelected? onRemoveCallback)
        {
            if (MerchantChanges.IsPopulatingShop)
            {
                HandleUpgradeIcon(__instance, artifactModel.ArtifactName);
            }

            if (artifactModel.baseId.StartsWith("Token") &&
                __instance.GetComponentInParent<RogueMerchantPanel>().Is(out var panel))
            {
                if (onAddCallback != null)
                {
                    var button = __instance.stackCountObj.GetComponentOrAdd<Button>();
                    button.targetGraphic = __instance.stackCountObj.GetComponentInChildren<Image>();
                    button.SetOnClick(() =>
                    {
                        var count = __instance.StackCount;
                        var ourPower = panel.GetTotalPower(panel.selectedInventoryArtifactIcons, false);
                        var merchantPower = panel.GetTotalPower(panel.selectedMerchantArtifactIcons, true);

                        if (merchantPower > ourPower && artifactModel.baseId == "Token")
                        {
                            for (var i = 0; i < Math.Min(merchantPower - ourPower, count); i++)
                            {
                                onAddCallback.Invoke(__instance);
                            }
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                onAddCallback.Invoke(__instance);
                            }

                        }
                    });
                }

                if (onRemoveCallback != null)
                {
                    var button = __instance.stackCountObj.GetComponentOrAdd<Button>();
                    button.targetGraphic = __instance.stackCountObj.GetComponentInChildren<Image>();
                    button.SetOnClick(() =>
                    {
                        var count = __instance.StackCount;
                        var ourPower = panel.GetTotalPower(panel.selectedInventoryArtifactIcons, false);
                        var merchantPower = panel.GetTotalPower(panel.selectedMerchantArtifactIcons, true);

                        if (ourPower > merchantPower && artifactModel.baseId == "Token")
                        {
                            for (var i = 0; i < Math.Min(ourPower - merchantPower, count); i++)
                            {
                                onRemoveCallback.Invoke(__instance);
                            }
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                onRemoveCallback.Invoke(__instance);
                            }
                        }

                    });
                }
            }
        }
    }

}