using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Legends;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.GameOver;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;

namespace RogueRemix;

public static class RulesChanges
{
    public static void ModifyRules(RogueData data)
    {
        var rules = data.rogueGameModeRules;

        if (RogueRemixMod.BoostsInShop)
        {
            // rules.bossRushHealthMultiplier = 1.05f;
        }

        // rules.bossHealthMultiplierPerStage = 5;

        foreach (var modeRule in rules.modeRules)
        {
            switch (modeRule.rogueTileType)
            {
                case RogueTileType.pathStandardGame:
                    break;
                case RogueTileType.boss:
                    break;
                case RogueTileType.miniBoss:
                    break;
                case RogueTileType.miniGame:
                    break;
                case RogueTileType.challenge:
                    break;
                case RogueTileType.none:
                    break;
            }
        }

        var ignoreCountArtifacts = RogueData.ignoreCountArtifacts.ToList();
        foreach (var (id, artifact) in GameData.Instance.artifactsData.artifactDatas)
        {
            if (artifact.ArtifactModel().Cast<ArtifactModelBase>().IsBoost)
            {
                ignoreCountArtifacts.Add(id);
            }
        }
        foreach (var modBoostArtifact in ModContent.GetContent<ModBoostArtifact>())
        {
            ignoreCountArtifacts.AddRange(modBoostArtifact.Ids);
        }

        RogueData.ignoreCountArtifacts = ignoreCountArtifacts.ToArray();
    }

    [HarmonyPatch(typeof(RogueVictoryScreen), nameof(RogueVictoryScreen.Open))]
    internal static class RogueVictoryScreen_Open
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueVictoryScreen __instance)
        {
            var rogueData = InGameData.CurrentGame.rogueData;
            var rogueLootData = __instance.RogueSaveData.rogueLootData;
            if (RogueRemixMod.BloonEncounterRewardsRemix &&
                RogueMap.TypeEarnsTokenAfterGame(rogueData.tileType))
            {
                rogueLootData.isTokenLoot = false;
                // rogueLootData.allowArtifacts = true;
                rogueLootData.allowTowers = true;
            }

            if (rogueData.tileType == RogueTileType.miniBoss)
            {
                // rogueLootData.bossType = rogueData.boss.ToString();
            }
        }
    }
}