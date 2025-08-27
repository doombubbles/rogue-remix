using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace RogueRemix.NewArtifacts;

public class Dreadnought : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .15f,
        Rare => .2f,
        Legendary => .3f,
        _ => 0f,
    };

    public override string Description(int tier) =>
        $"Monkey Buccaneers deal {Effect(tier):P0} more damage by shooting molten cannonballs that can pop Frozen and Lead Bloons";

    public override string InstaMonkey(int tier) => TowerType.MonkeyBuccaneer;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Legendary => [2, 0, 0],
        Rare => [2, 1, 0],
        _ => [1, 1, 0]
    };

    public override string Icon => VanillaSprites.FireballUpgradeIcon;
    public override bool SmallIcon => true;

    public override TowerSet RarityFrameType => TowerSet.Military;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new DamageBoostBehaviorModel("", 1 + Effect(artifactModel.tier)));
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var display = gameModel.GetTower(TowerType.WizardMonkey, 0, 1, 0)
            .GetAttackModel("Fireball")
            .GetDescendant<DisplayModel>()
            .display;

        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.MonkeyBuccaneer).AsIEnumerable())
        {
            towerModel.GetDescendants<ProjectileModel>().ForEach(proj =>
            {
                if (proj.HasBehavior<CreateProjectileOnContactModel>() ||
                    !proj.HasBehavior<TravelStraitModel>() ||
                    !proj.HasBehavior(out DamageModel damage)) return;

                damage.immuneBloonPropertiesOriginal &= ~BloonProperties.Lead;
                damage.immuneBloonProperties &= ~BloonProperties.Lead;
                damage.immuneBloonPropertiesOriginal &= ~BloonProperties.Frozen;
                damage.immuneBloonProperties &= ~BloonProperties.Frozen;

                proj.SetDisplay(display);
                proj.scale = proj.radius > 3 ? 1f : .7f;

                if (proj.HasBehavior<AddBehaviorToBloonModel>() && proj.HasDescendant(out DamageOverTimeModel dot))
                {
                    dot.triggerImmediate = true;
                }
            });
        }
    }
}