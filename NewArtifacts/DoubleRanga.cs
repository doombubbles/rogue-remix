using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;

namespace RogueRemix.NewArtifacts;

public class DoubleRanga : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .7f,
        Rare => .6f,
        Legendary => .4f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Boomerang Monkeys throw 2 Boomerangs at once but have {Effect(tier):P0} increased reload time";

    public override string InstaMonkey(int tier) => TowerType.BoomerangMonkey;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [1, 0, 0],
        _ => [2, 0, 0]
    };

    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Primary;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new RateBoostBehaviorModel("", 1 + Effect(artifactModel.tier)));
    }


    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.BoomerangMonkey))
        {
            var attacks = new List<AttackModel> {towerModel.GetAttackModel()};
            if (towerModel.appliedUpgrades.Contains(UpgradeType.MOABPress))
            {
                attacks.Add(towerModel.GetAttackModel("MOABPress"));
            }

            foreach (var weapon in attacks.SelectMany(attack => attack.weapons))
            {
                weapon.SetEmission(new ArcEmissionModel("", 2, 0, 30, null, false, false));
            }

            if (towerModel.HasBehavior(out OrbitModel orbitModel))
            {
                orbitModel.count *= 2;
            }
        }
    }
}