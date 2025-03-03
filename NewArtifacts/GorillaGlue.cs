using System.Collections.Generic;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Artifacts;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class GorillaGlue : ModItemArtifact, IArtifactSynergy
{
    public static float Effect(int tier) => tier switch
    {
        Common => .2f,
        Rare => .3f,
        Legendary => .5f,
        _ => 0,
    };

    public override string Description(int tier) =>
        $"Glue effects slow Bloons down {Effect(tier):P0} more, and damage Bloons {Effect(tier) * 10:N0}x faster";

    public override string InstaMonkey(int tier) => TowerType.GlueGunner;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Legendary => [0, 2, 0],
        Rare => [1, 2, 0],
        _ => [1, 1, 0]
    };

    public override string Icon => VanillaSprites.TubeOfAmazoGlue;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Primary;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var models = new List<Model>();

        models.AddRange(gameModel.GetTowersWithBaseId(TowerType.GlueGunner));
        models.Add(gameModel.GetPowerWithName("GlueTrap"));
        models.Add(gameModel.GetGeraldoItemWithName("TubeOfAmazoGlue"));

        foreach (var model in models)
        {
            model.GetDescendants<SlowModel>().ForEach(slowModel => Apply(slowModel, tier));
            model.GetDescendants<DamageOverTimeModel>().ForEach(dotModel => Apply(dotModel, tier));
        }
    }

    public static void Apply(SlowModel model, int tier)
    {
        model.Multiplier *= 1 - Effect(tier);
    }

    public static void Apply(DamageOverTimeModel model, int tier)
    {
        model.Interval /= Effect(tier) * 10;
    }

    public void ModifyOtherArtifacts(List<ArtifactModelBase> artifacts, int tier)
    {
        foreach (var artifact in artifacts)
        {
            if (artifact.baseId != "CorrosiveTorpEDarts") continue;

            var slowModel = artifact.GetDescendant<SlowModel>();
            var dotModel = artifact.GetDescendant<AddBehaviorToBloonModel>().behaviors[0]!.Cast<DamageOverTimeModel>();

            slowModel.Multiplier =
                IArtifactSynergy.RestoreStore(slowModel.Multiplier, Name + artifact.ArtifactName + "Multiplier");
            dotModel.Interval =
                IArtifactSynergy.RestoreStore(slowModel.Multiplier, Name + artifact.ArtifactName + "Interval");

            Apply(slowModel, tier);
            Apply(dotModel, tier);
        }

    }
}