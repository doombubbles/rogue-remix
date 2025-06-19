using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppSystem.IO;

namespace RogueRemix.NewArtifacts;

public class Overtime : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .5f,
        Rare => .7f,
        Legendary => 1,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Engineers and Sentries permanently have a {Effect(tier):P0} effectiveness Overlock applied to them ({Effect(tier) * .4:P0} reduced reload time)";

    public override string Icon => VanillaSprites.OverclockUpgradeIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Support;

    public override string InstaMonkey(int tier) => TowerType.EngineerMonkey;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [0, 1, 1],
        Rare => [0, 2, 0],
        Legendary => [0, 2, 0],
        _ => []
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    [HarmonyPatch(typeof(InGame), nameof(InGame.Update))]
    internal static class InGame_Update
    {
        [HarmonyPostfix]
        internal static void Postfix(InGame __instance)
        {
            if (RogueLegendsManager.instance?.RogueSaveData != null &&
                __instance.bridge?.Simulation != null &&
                RogueLegendsManager.instance.RogueSaveData.HasArtifact<Overtime>(out var tier))
            {
                foreach (var tower in __instance.bridge.Simulation.towerManager.GetTowers().ToArray())
                {
                    var baseId = tower.towerModel.baseId;
                    if (baseId != TowerType.EngineerMonkey && !baseId.Contains(TowerType.Sentry) ||
                        tower.GetMutatorById("Overclock") != null) continue;

                    var overclock = Game.instance.model
                        .GetTower(TowerType.EngineerMonkey, 0, 4)
                        .GetAbility()
                        .GetBehavior<OverclockModel>()
                        .Duplicate();

                    overclock.rateModifier = 1 - (1 - overclock.rateModifier) * Effect(tier);

                    tower.AddMutator(overclock.Mutator, -1, false);
                }
            }

        }
    }
}