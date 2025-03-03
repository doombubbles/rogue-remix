using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace RogueRemix.NewArtifacts;

public class FasterFasteners : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .15f,
        Rare => .2f,
        Legendary => .35f,
        _ => 0,
    };

    public override string Description(int tier) =>
        $"Tack Shooters' Tacks and Engineers' Nails travel {Effect(tier):P0} faster, and they have {Effect(tier):P0} increased attack speed";

    public override string Icon => VanillaSprites.EvenMoreTacksUpgradeIcon;
    public override bool SmallIcon => true;

    public override string InstaMonkey(int tier) => tier == Rare ? TowerType.TackShooter : TowerType.EngineerMonkey;

    public override int[] InstaTiers(int tier) => tier == Legendary ? [0, 0, 3] : [2, 0, 2];

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehaviors([
            new ProjectileSpeedBoostBehaviorModel("", 1 + Effect(artifactModel.tier)),
            new RateBoostBehaviorModel("", 1 / (1 + Effect(artifactModel.tier)))
        ], boost => boost.towerTypes = new Il2CppStringArray([TowerType.TackShooter, TowerType.EngineerMonkey]));
    }
}