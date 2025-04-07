using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Main.HeroSelect;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace RogueRemix;

public static class Completionism
{
    [HarmonyPatch(typeof(LegendsManager), nameof(LegendsManager.IncreaseRogueStage))]
    internal static class LegendsManager_IncreaseRogueStage
    {
        [HarmonyPostfix]
        internal static void Postfix(LegendsManager __instance)
        {
            if (!RogueRemixMod.TrackHeroLoadoutCompletions) return;

            var saveData = __instance.RogueSaveData;
            var stats = __instance.LegendsData.savedLegendsStats;
            stats.TryAdd(nameof(RogueRemix), new Dictionary<string, bool>());

            var rogueRemixStats = stats[nameof(RogueRemix)];

            rogueRemixStats[saveData.selectedHeroSkin + "Bronze"] = true;
            if (saveData.stage >= 2)
            {
                rogueRemixStats[saveData.selectedHeroSkin + "Silver"] = true;
            }
            if (saveData.stage >= 4)
            {
                rogueRemixStats[saveData.selectedHeroSkin + "Gold"] = true;
                if (saveData.isChimps)
                {
                    rogueRemixStats[saveData.selectedHeroSkin + "Black"] = true;
                }
            }
            Game.Player.Save();
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
            if (Game.Player.Data.LegendsData.savedLegendsStats.TryGetValue(nameof(RogueRemix), out var stats))
            {
                image.color = new Color(1, 1, 1);
                if (stats.GetValueOrDefault(heroId + "Black"))
                {
                    image.SetSprite(VanillaSprites.MainBgPanelHematite);
                    return;
                }
                if (stats.GetValueOrDefault(heroId + "Gold"))
                {
                    image.SetSprite(VanillaSprites.MainBGPanelGold);
                    return;
                }
                if (stats.GetValueOrDefault(heroId + "Silver"))
                {
                    image.SetSprite(VanillaSprites.MainBGPanelSilver);
                    return;
                }
                if (stats.GetValueOrDefault(heroId + "Bronze"))
                {
                    image.SetSprite(VanillaSprites.MainBGPanelBronze);
                    return;
                }
            }

            image.color = new Color(0.706f, 0.663f, 0.588f);
            image.SetSprite(VanillaSprites.InsertPanelWhiteRound);
        }
    }

}