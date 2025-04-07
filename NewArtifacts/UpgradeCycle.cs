using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;

namespace RogueRemix.NewArtifacts;

public class UpgradeCycle : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .1f,
        Rare => .15f,
        Legendary => .25f,
        _ => 0f,
    };

    public override string Description(int tier) => $"Upgrades for towers are {Effect(tier):P0} cheaper";

    public override string Icon => VanillaSprites.BananaSalvageUpgradeIcon;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new DiscountBoostBehaviorModel("", 1 - Effect(artifactModel.tier)));
    }
}