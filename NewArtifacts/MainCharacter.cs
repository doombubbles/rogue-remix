using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class MainCharacter : ModItemArtifact
{
    private static float Effect(int tier) => .2f - tier * .1f;

    public override string Description(int tier) =>
        "Heroes now count as Primary, Military, Magic, and Support towers." +
        tier switch
        {
            Legendary => "",
            _ => $" Hero attack range reduced by {Effect(tier):P0}"
        };

    public override string Icon => VanillaSprites.HeroesIcon;
    // public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBehavior(new CountAllCategoriesBehaviorModel("",
            new Il2CppStructArray<TowerSet>([TowerSet.Hero]),
            new Il2CppStructArray<TowerSet>([TowerSet.Primary, TowerSet.Military, TowerSet.Magic, TowerSet.Support])));

        artifactModel.AddBoostBehavior(new RangeBoostBehaviorModel("", 1 - Effect(artifactModel.tier)), boost =>
            boost.towerSets = new Il2CppStructArray<TowerSet>([TowerSet.Hero]));
    }
}