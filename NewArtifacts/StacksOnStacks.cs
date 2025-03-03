using System;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;

namespace RogueRemix.NewArtifacts;

public class StacksOnStacks : ModItemArtifact
{
    public override string DescriptionCommon => "All tower buff zones can stack up to 2x their normal limit";
    public override string DescriptionRare => "All tower buff zones can stack up to 3x their normal limit";
    public override string DescriptionLegendary => "All tower buff zones can stack up to 5x their normal limit";

    public override string InstaMonkey(int tier) => TowerType.MonkeyVillage;

    public override int[] InstaTiers(int tier) => [tier, 0, 0];

    public override string Icon => VanillaSprites.JungleDrumsUpgradeIcon;
    public override bool SmallIcon => true;

    public override TowerSet RarityFrameType => TowerSet.Support;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var size = tier switch
        {
            Legendary => 5,
            Rare => 3,
            _ => 2
        };

        foreach (var towerModel in gameModel.towers)
        {
            towerModel.GetDescendants<SupportModel>().ForEach(supportModel =>
            {
                supportModel.maxStackSize = size * Math.Max(supportModel.maxStackSize, 1);

                supportModel.GetIl2CppType()
                    .GetField("isUnique", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    ?.SetValue(supportModel, false);

                if (supportModel.GetBuffIndicatorModel().Is(out var buffIndicatorModel))
                {
                    buffIndicatorModel.stackable = true;
                    buffIndicatorModel.maxStackSize = size * Math.Max(buffIndicatorModel.maxStackSize, 1);
                }
            });
        }
    }
}