using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class GottaGoFast : ModItemArtifact
{
    public override string Description(int tier) =>
        $"All Monkeys with a \"Faster _____\" upgrade attack an extra {Speed(tier):P0} faster";

    public override string Icon => VanillaSprites.FasterDartsUpgradeIcon;
    public override bool SmallIcon => true;

    private static float Speed(int tier) => tier switch
    {
        Common => .15f,
        Rare => .2f,
        Legendary => .35f,
        _ => 0f,
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        var towerUpgrades = Game.instance.model.towers.GroupBy(model => model.baseId)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(model => model.appliedUpgrades).Distinct().ToArray()
            );

        foreach (var (tower, upgrades) in towerUpgrades)
        {
            var tiers = new[] {6, 6, 6};
            var any = false;

            foreach (var upgrade in upgrades.Where(s => s.Contains("Faster")))
            {
                var upgradeModel = Game.instance.model.GetUpgrade(upgrade);
                tiers[upgradeModel.path] = upgradeModel.tier + 1;
                any = true;
            }

            if (!any) continue;

            artifactModel.AddBoostBehavior(new RateBoostBehaviorModel("", 1 / (1 + Speed(artifactModel.tier))), boost =>
            {
                boost.towerTypes = new Il2CppStringArray([tower]);
                boost.tiers = tiers;
            });
        }
    }
}