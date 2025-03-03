using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class HeavyHitter : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .25f,
        Rare => .35f,
        Legendary => .5f,
        _ => 0
    };

    private static float Interval(int tier) => tier switch
    {
        Common => .3f,
        Rare => .2f,
        Legendary => .1f,
        _ => 1
    };

    public override string Description(int tier) =>
        $"Dart Monkey projectiles move 50% slower but they last {Effect(tier):P0} longer and are able to rehit the same Bloon every {Interval(tier):N1} seconds";

    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Primary;

    public override string InstaMonkey(int tier) => TowerType.DartMonkey;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [1, 0, 1],
        Rare => [2, 0, 1],
        Legendary => [2, 0, 0],
        _ => []
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new ProjectileLifespanBoostBehaviorModel("", 1 + Effect(artifactModel.tier)),
            model => model.towerTypes = new Il2CppStringArray([TowerType.DartMonkey]));
        artifactModel.AddBoostBehavior(new ProjectileSpeedBoostBehaviorModel("", -.5f),
            model => model.towerTypes = new Il2CppStringArray([TowerType.DartMonkey]));
        artifactModel.AddProjectileBehavior(new ClearHitBloonsModel("", Interval(artifactModel.tier)),
            model => model.towerTypes = new Il2CppStringArray([TowerType.DartMonkey]));
    }

}