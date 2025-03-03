using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class EnchantedWeapons : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .1f,
        Rare => .2f,
        Legendary => .3f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Ninja Monkeys' Shurikens and Mermonkeys' Tridents can hit all Bloon types, and they have {Effect(tier):P0} increased damage and pierce";

    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Magic;

    public override string InstaMonkey(int tier) => tier == Rare ? TowerType.Mermonkey : TowerType.NinjaMonkey;

    public override int[] InstaTiers(int tier) => tier == Legendary ? [3, 0, 0] : [2, 0, 0];

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehaviors([
            new DamageBoostBehaviorModel("", 1 + Effect(artifactModel.tier)),
            new PierceBoostBehaviorModel("", 1 + Effect(artifactModel.tier))
        ], boost => boost.towerTypes = new Il2CppStringArray([TowerType.NinjaMonkey, TowerType.Mermonkey]));
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var tower in gameModel.towers)
        {
            if (tower.baseId == TowerType.NinjaMonkey || tower.baseId == TowerType.Mermonkey)
            {
                tower.GetAttackModel().GetDescendants<DamageModel>().ForEach(damage =>
                {
                    damage.immuneBloonProperties = BloonProperties.None;
                });
            }

        }
    }
}