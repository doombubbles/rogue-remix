using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.GameOver;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppSystem.IO;

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

    public static void ApplyDamageToModifiers(Model model, float multiplier)
    {
        if (!RogueRemixMod.DamageBoostsAffectModifiers) return;

        foreach (var modifier in model.GetDescendants<DamageModifierModel>().ToArray())
        {
            if (modifier.Is(out DamageModifierForTagModel tag))
            {
                tag.damageAddative *= multiplier;
            }
            else if (modifier.Is(out DamageModifierForBloonTypeModel bloonType))
            {
                bloonType.damageAdditive *= multiplier;
            }
            else if (modifier.Is(out DamageModifierForBloonStateModel bloonState))
            {
                bloonState.damageAdditive *= multiplier;
            }
            else if (modifier.Is(out DamageModifierForModifiersModel modifiers))
            {
                modifiers.damageAddative *= multiplier;
            }
            else if (modifier.Is(out DamageModifierForRoundModel round))
            {
                round.damagePerRound *= multiplier;
            }
        }

        /*foreach (var bonus in model.GetDescendants<AddBonusDamagePerHitToBloonModel>().ToArray())
        {
            bonus.perHitDamageAddition *= multiplier;
        }*/
    }

    /// <summary>
    /// Make Damage boosts also affect additive damage modifiers on towers
    /// </summary>
    [HarmonyPatch(typeof(DamageBoostBehavior.DamageBoostMutator),
        nameof(DamageBoostBehavior.DamageBoostMutator.Mutate))]
    internal static class DamageBoostMutator_Mutate
    {
        [HarmonyPostfix]
        internal static void Postfix(DamageBoostBehavior.DamageBoostMutator __instance, Model model)
        {
            ApplyDamageToModifiers(model, __instance.multiplier);
        }
    }
}