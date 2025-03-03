using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class PirateCove : ModItemArtifact
{
    public override string DescriptionCommon =>
        "The Crow's Nest upgrade gives Camo detection to all towers within the Monkey Buccaneer's range";
    public override string DescriptionRare =>
        "The Crow's Nest upgrade gives Camo detection and 15% Pierce to all towers within the Monkey Buccaneer's range";
    public override string DescriptionLegendary =>
        "The Crow's Nest upgrade gives Camo detection and 30% Pierce to all towers within the Monkey Buccaneer's range";

    public override string InstaMonkey(int tier) => TowerType.MonkeyBuccaneer;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Legendary => [0, 0, 2],
        Rare => [0, 1, 2],
        _ => [0, 1, 1]
    };

    public override string Icon => VanillaSprites.CrowsNestUpgradeIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Military;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddTowerBehaviors([
            new VisibilitySupportModel("", true, "PirateCove:Visibility", false, null,
                "", "").ApplyBuffIcon<PirateCoveBuff>(),
            new PiercePercentageSupportModel("", true, 1 + artifactModel.tier * .15f, "PirateCove:Pierce", null,
                false, "", "")
            {
                appliesToOwningTower = true,
                maxStackSize = 1
            }.ApplyBuffIcon<PirateCoveBuff>()
        ], model =>
        {
            model.towerTypes = new Il2CppStringArray([TowerType.MonkeyBuccaneer]);
            model.tiers = new Il2CppStructArray<int>([6, 6, 2]);
        });
    }
}

public class PirateCoveBuff : ModBuffIcon
{
    public override string Icon => VanillaSprites.CrowsNestUpgradeIcon;
}