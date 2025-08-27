using System.Collections.Generic;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class TrueArmorPiercing : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => 2f,
        Rare => 2.5f,
        Legendary => 3,
        _ => 1f
    };

    public override string Description(int tier) =>
        $"All Monkey Subs can pop Lead Bloons and deal {Effect(tier)}x damage to Lead, Ceramic, and Fortified Bloons (includes DDTs)";

    public override string Icon => VanillaSprites.ArmorPiercingDartsUpgradeIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Military;

    public override string InstaMonkey(int tier) => TowerType.MonkeySub;

    public override int[] InstaTiers(int tier) => [0, 0, tier + 2];

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddProjectileBehavior(
            new DamageModifierForTagModel("",
                BloonTag.Lead + "," + BloonTag.Ddt + "," + BloonTag.Ceramic + "," + BloonTag.Fortified,
                Effect(artifactModel.tier), 0, false, false),
            boost =>
            {
                boost.towerTypes = new Il2CppStringArray([TowerType.MonkeySub]);
            });
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.MonkeySub).AsIEnumerable())
        {
            towerModel.GetDescendants<DamageModel>()
                .ForEach(model => model.immuneBloonProperties &= ~BloonProperties.Lead);
        }
    }
}