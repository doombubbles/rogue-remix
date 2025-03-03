using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class PartMonkeyPartMachine : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Legendary => .4f,
        Rare => .25f,
        _ => .15f,
    };

    public override string Description(int tier) =>
        $"Bionic Boomers and Robo Monkeys have {Effect(tier):P0} increased damage and ability cooldown rate";

    public override string InstaMonkey(int tier) => tier == Rare ? TowerType.SuperMonkey : TowerType.BoomerangMonkey;

    public override int[] InstaTiers(int tier) => tier == Legendary ? [0, 4, 0] : [0, 3, 0];

    public override string Icon => VanillaSprites.TechTerrorUpgradeIconAA;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehaviors([
            new CooldownBoostBehaviorModel("", 1 + Effect(artifactModel.tier)),
            new DamageBoostBehaviorModel("", 1 + Effect(artifactModel.tier))
        ], boost =>
        {
            boost.towerTypes = new Il2CppStringArray([TowerType.BoomerangMonkey, TowerType.SuperMonkey]);
            boost.tiers = new Il2CppStructArray<int>([6, 3, 6]);
        });
    }
}