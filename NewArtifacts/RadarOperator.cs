using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Unity;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class RadarOperator : ModItemArtifact
{
    public override string DescriptionCommon =>
        "Heroes give Radar Scanner buff to nearby Monkeys (can see Camo)";
    public override string DescriptionRare =>
        "Heroes give Radar Scanner buff to nearby Monkeys (can see Camo) and these monkeys have 15% increased range";
    public override string DescriptionLegendary =>
        "Heroes give Radar Scanner buff to nearby Monkeys (can see Camo) and these monkeys have 30% increased range";

    public override bool SmallIcon => true;
    public override string Icon => VanillaSprites.RadarScannerUpgradeIcon;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddTowerBehaviors([
                Game.instance.model.GetTower(TowerType.MonkeyVillage, 0, 2, 0)
                    .GetBehavior<VisibilitySupportModel>()
                    .Duplicate(),
                new RangeSupportModel("", true,
                    artifactModel.tier * .15f, 0, "RadarOperator", null, false, "", "")
            ],
            model => model.towerSets = new Il2CppStructArray<TowerSet>([TowerSet.Hero]));
    }
}