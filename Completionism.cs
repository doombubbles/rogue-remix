using System.Collections.Generic;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Main.HeroSelect;
using UnityEngine;
using UnityEngine.UI;

namespace RogueRemix;

public static class Completionism
{
    public static readonly Dictionary<string, bool> RogueRemixStats = new();

    [HarmonyPatch(typeof(RogueLegendsManager), nameof(RogueLegendsManager.IncreaseRogueStage))]
    internal static class RogueLegendsManager_IncreaseRogueStage
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueLegendsManager __instance)
        {
            if (!RogueRemixMod.TrackHeroLoadoutCompletions) return;

            var saveData = __instance.RogueSaveData;

            RogueRemixStats[saveData.selectedHeroSkin + "Bronze"] = true;
            if (saveData.stage >= 2)
            {
                RogueRemixStats[saveData.selectedHeroSkin + "Silver"] = true;
            }
            if (saveData.stage >= 4)
            {
                RogueRemixStats[saveData.selectedHeroSkin + "Gold"] = true;
                if (saveData.isChimps)
                {
                    RogueRemixStats[saveData.selectedHeroSkin + "Black"] = true;
                }
            }

            ModContent.GetInstance<RogueRemixMod>().SaveModSettings();
        }
    }

    [HarmonyPatch(typeof(HeroUpgradeDetails), nameof(HeroUpgradeDetails.DisplayRogueStarterKit))]
    internal static class HeroUpgradeDetails_DisplayRogueStarterKit
    {
        [HarmonyPostfix]
        internal static void Postfix(HeroUpgradeDetails __instance, string heroId)
        {
            if (!RogueRemixMod.TrackHeroLoadoutCompletions) return;

            var image = __instance.rogueLoadoutObj.GetComponent<Image>();

            image.color = new Color(1, 1, 1);
            if (RogueRemixStats.GetValueOrDefault(heroId + "Black"))
            {
                image.SetSprite(VanillaSprites.MainBgPanelHematite);
                return;
            }
            if (RogueRemixStats.GetValueOrDefault(heroId + "Gold"))
            {
                image.SetSprite(VanillaSprites.MainBGPanelGold);
                return;
            }
            if (RogueRemixStats.GetValueOrDefault(heroId + "Silver"))
            {
                image.SetSprite(VanillaSprites.MainBGPanelSilver);
                return;
            }
            if (RogueRemixStats.GetValueOrDefault(heroId + "Bronze"))
            {
                image.SetSprite(VanillaSprites.MainBGPanelBronze);
                return;
            }

            image.color = new Color(0.706f, 0.663f, 0.588f);
            image.SetSprite(VanillaSprites.InsertPanelWhiteRound);
        }
    }

}