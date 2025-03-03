using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
namespace RogueRemix.NewArtifacts;

public class PrecisionMunition : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .1f,
        Rare => .2f,
        Legendary => .40f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Bomb Shooters have {Effect(tier):P0} more pierce and global range. Bombs instantly explode at the target's location";

    public override bool SmallIcon => true;

    public override string InstaMonkey(int tier) => TowerType.BombShooter;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [1, 1, 0],
        Rare => [2, 1, 0],
        Legendary => [2, 0, 0],
        _ => []
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new PierceBoostBehaviorModel("", 1 + Effect(artifactModel.tier)),
            model => model.towerTypes = new Il2CppStringArray([TowerType.BombShooter]));
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.BombShooter))
        {
            towerModel.isGlobalRange = true;
            towerModel.range = 20;

            towerModel.GetDescendants<AttackModel>().ForEach(attack =>
            {
                var weapon = attack.weapons[0];
                var projectile = weapon?.projectile;

                if (weapon != null &&
                    projectile != null &&
                    weapon.emission.Is<SingleEmissionModel>() &&
                    projectile.HasBehavior<TravelStraitModel>() &&
                    Mathf.Approximately(projectile.maxPierce, 1))
                {
                    attack.range = 9999999;
                    weapon.SetEmission(new InstantDamageEmissionModel("", null));
                    projectile.RemoveBehavior<TravelStraitModel>();
                    projectile.RemoveBehavior<TrackTargetModel>();
                    projectile.AddBehavior(new InstantModel("", false, false, false));
                    projectile.AddBehavior(new AgeModel("", .1f, 0, false, null));
                }
            });
        }
    }
}