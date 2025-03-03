using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Unity;
using Il2CppSystem.Linq;

namespace RogueRemix.NewArtifacts;

public class LaserSharp : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .25f,
        Rare => .35f,
        Legendary => .5f,
        _ => 0f,
    };

    public override string Description(int tier) =>
        $"Super Monkeys with Laser Blasts (but not Plasma Blasts) apply Laser Shock. All Lasers deal {Effect(tier):P0} more damage to Shocked Bloons and can hit all Bloon types";

    public override string Icon => VanillaSprites.LaserShockUpgradeIcon;
    public override bool SmallIcon => true;

    public override string InstaMonkey(int tier) => tier == Rare ? TowerType.SuperMonkey : TowerType.DartlingGunner;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Legendary => [3, 0, 0],
        Rare => [1, 0, 0],
        _ => [2, 0, 0]
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var towerModel in gameModel.towers)
        {
            var laserBlasts = towerModel.baseId == TowerType.SuperMonkey &&
                              towerModel.tiers[0] == 1 &&
                              towerModel.tiers[1] < 4;
            var laserCannon = towerModel.baseId == TowerType.DartlingGunner && towerModel.tiers[0] >= 3;
            var laserSentry = towerModel.baseId == TowerType.SentryEnergy;
            var apachePrime = towerModel.baseId == TowerType.HeliPilot && towerModel.tiers[0] >= 5;

            if (laserBlasts)
            {
                var laser = Game.instance.model
                    .GetTower(TowerType.DartlingGunner, Math.Max(towerModel.tier, 2), 0, 0)
                    .GetDescendant<ProjectileModel>();

                towerModel.GetDescendants<ProjectileModel>().ForEach(proj =>
                {
                    proj.AddBehavior(laser.GetBehavior<AddBehaviorToBloonModel>().Duplicate());
                    proj.AddBehavior(laser.GetBehavior<DamageModifierForBloonStateModel>().Duplicate());
                    proj.hasDamageModifiers = true;
                });
            }

            if (laserBlasts || laserCannon || laserSentry || apachePrime)
            {
                foreach (var proj in towerModel.GetDescendants<ProjectileModel>().ToArray())
                {
                    if (apachePrime && proj.display?.AssetGUID != "ffa3be03eb9b2d24da77aeff09693b00") continue;

                    proj.AddBehavior(new DamageModifierForBloonStateModel("",
                        "LaserShock1,LaserShock2,LaserShock3,LaserShock4", 1 + Effect(tier), 0, false, false, false));
                    proj.hasDamageModifiers = true;

                    if (proj.HasBehavior(out DamageModel damageModel))
                    {
                        damageModel.immuneBloonProperties = BloonProperties.None;
                    }
                }
            }
        }
    }
}