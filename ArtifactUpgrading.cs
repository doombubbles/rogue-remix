﻿using System.Linq;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using RogueRemix.NewArtifacts;
using UnityEngine;

namespace RogueRemix;

public static class ArtifactUpgrading
{
    public static bool PopulatingOwned { get; private set; }

    public static void HandleUpgradeIcon(RogueArtifactDisplayIcon icon, string artifactName)
    {
        if (!RogueRemixMod.ArtifactUpgrading ||
            LegendsManager.instance?.RogueSaveData == null ||
            artifactName.Contains("BoostArtifact")) return;

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
                              LegendsManager.instance.RogueSaveData.artifactsInventory.Any(loot =>
                                  loot.artifactName == artifactName));
    }

    /// <summary>
    /// Allow duplicates of the same tier to appear
    /// </summary>
    [HarmonyPatch(typeof(LegendsManager), nameof(LegendsManager.CheckIfArtifactOwned))]
    internal static class LegendsManager_CheckIfArtifactOwned
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

            if (artifact.artifactName.Contains("BoostArtifact"))
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
        internal static void Postfix(RogueArtifactDisplayIcon __instance, ArtifactModelBase artifactModel)
        {
            if (MerchantChanges.IsPopulatingShop)
            {
                HandleUpgradeIcon(__instance, artifactModel.ArtifactName);
            }
        }
    }

}