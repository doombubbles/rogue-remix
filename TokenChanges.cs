using System;
using System.Linq;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppSystem.Collections.Generic;
using RogueRemix.NewArtifacts;

namespace RogueRemix;

public static class TokenChanges
{
    private static bool hijackingLootButtonClicked;
    private static RogueLoot? lastRogueLoot;

    public static void AddTokenFallback<T>(List<T> results) where T : RogueLoot
    {
        var artifacts = results.ToArray().OfIl2CppType<ArtifactLoot>()
            .Where(loot => loot.lootType == RogueLootType.permanent)
            .ToArray();

        if (artifacts.Length == 0) return;

        var tokenAmount = artifacts.Max(loot => GameData.Instance.artifactsData
            .GetArtifactData(loot.artifactName)
            .ArtifactModel()
            .Cast<ArtifactModelBase>()
            .ArtifactPower);

        if (artifacts.FirstOrDefault(loot => loot.baseId == "Token").Is(out var tokenLoot))
        {
            tokenLoot!.tier = Math.Max(tokenAmount, tokenLoot.tier);
        }
        else
        {
            results.Add(new ArtifactLoot
            {
                artifactName = "Token",
                tier = tokenAmount,
                lootType = RogueLootType.permanent,
                baseId = "Token",
                startingArtifact = false
            }.Cast<T>());
        }
    }

    public static void RewardTokens(int amount, PopupScreen.ReturnCallback? callback = null)
    {
        var tokenLoot = new ArtifactLoot
        {
            lootType = RogueLootType.permanent,
            artifactName = "Token",
            baseId = "Token"
        };
        for (var i = 0; i < amount; i++)
        {
            LegendsManager.instance.RogueSaveData.AddArtifactToInventory(tokenLoot, true);
        }

        PopupScreen.instance.ShowRogueRewardPopup(callback ?? new Action(() => { }), tokenLoot, false, amount);
    }

    /// <summary>
    /// Add fallback tokens to random loot
    /// </summary>
    [HarmonyPatch(typeof(LegendsManager), nameof(LegendsManager.GetRandomLoot))]
    internal static class LegendsManager_GetRandomLoot
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueLootData rogueLootData, ref List<RogueLoot> __result)
        {
            if (rogueLootData is
                {
                    allowArtifacts: true, rogueLootType: RogueLootType.permanent, isTokenLoot: false
                })
            {
                AddTokenFallback(__result);
            }
        }
    }

    /// <summary>
    /// Add fallback tokens to random artifacts
    /// </summary>
    [HarmonyPatch(typeof(LegendsManager), nameof(LegendsManager.GetRandomArtifacts))]
    internal static class LegendsManager_GetRandomArtifacts
    {
        [HarmonyPostfix]
        internal static void Postfix(LegendsManager __instance, ref List<ArtifactLoot> __result, RogueLootType type,
            int tileSeed, int guaranteedLegendary)
        {
            if (type != RogueLootType.permanent) return;

            if (MerchantChanges.IsPopulatingShop)
            {
                if (RogueRemixMod.BoostsRemix)
                {
                    var boosts = __instance.GetRandomArtifacts(3, RogueLootType.boost, 0, tileSeed,
                        guaranteedLegendary: guaranteedLegendary > 0 ? 1 : 0);
                    foreach (var boost in boosts)
                    {
                        boost.lootType = RogueLootType.permanent;
                    }
                    __result.AddRange(boosts.Cast<IEnumerable<ArtifactLoot>>());
                }
            }
            else
            {
                AddTokenFallback(__result);
            }
        }
    }

    /// <summary>
    /// Fix the stack count on RogueLootPopup screen
    /// </summary>
    [HarmonyPatch(typeof(RogueLootPopup), nameof(RogueLootPopup.SetLootChoices))]
    internal static class RogueLootPopup_SetLootChoices
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueLootPopup __instance)
        {
            var choices = __instance.activeButtons.ToArray()
                .Select(o => o.GetComponentInChildren<RogueArtifactDisplayIcon>())
                .Where(icon => icon != null)
                .ToArray();

            hijackingLootButtonClicked = true;
            foreach (var rogueArtifactDisplayIcon in choices)
            {
                lastRogueLoot = null;
                rogueArtifactDisplayIcon.selectBtn.onClick.Invoke();
                if (!lastRogueLoot.Is(out ArtifactLoot artifactLoot)) continue;

                if (artifactLoot.artifactName == "Token" && artifactLoot.tier > 0)
                {
                    rogueArtifactDisplayIcon.StackCount = artifactLoot.tier;
                }

                ArtifactUpgrading.HandleUpgradeIcon(rogueArtifactDisplayIcon, artifactLoot.artifactName);
            }
            hijackingLootButtonClicked = false;
        }
    }

    /// <summary>
    /// Hijack LootButtonClicked since RogueLoot classes aren't stored in the buttons after initializing
    /// </summary>
    [HarmonyPatch(typeof(RogueLootPopup), nameof(RogueLootPopup.LootButtonClicked))]
    internal static class RogueLootPopup_LootButtonClicked
    {
        [HarmonyPrefix]
        internal static bool Prefix(RogueLoot loot)
        {
            lastRogueLoot = loot;
            return !hijackingLootButtonClicked;
        }
    }

    /// <summary>
    /// Handle receiving the tokens
    /// </summary>
    [HarmonyPatch(typeof(RogueLootPopup), nameof(RogueLootPopup.AddArtifactAndClose))]
    internal static class RogueLootPopup_AddArtifact
    {
        [HarmonyPrefix]
        internal static bool Prefix(RogueLootPopup __instance, ArtifactLoot artifact)
        {
            if (artifact is {artifactName: "Token", tier: > 0})
            {
                RewardTokens(artifact.tier, __instance.onRewardScreenClosedCallback);
                LegendsManager.instance.CheckFeats();
                __instance.canHide = true;
                __instance.OKClicked();
                return false;
            }

            return true;
        }
    }
}