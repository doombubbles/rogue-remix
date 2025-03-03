using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppSystem.Linq;
using static Il2CppAssets.Scripts.Models.TowerSets.TowerSet;

namespace RogueRemix.ArtifactChanges;

public class BoostArtifactChange : ModVanillaArtifact
{
    private static readonly string[] TowerSets =
    [
        nameof(Primary), nameof(Military), nameof(Magic), nameof(Support), "AllTower"
    ];

    private static readonly string[] BoostTypes = ["Inspiration", "Precision", "Quickening"];

    public string TowerSet { get; init; } = null!;
    public string BoostType { get; init; } = null!;

    public override string Name => $"BoostArtifact{TowerSet}{BoostType}";

    public override string DisplayName => (TowerSet + BoostType).Spaced();

    public override IEnumerable<ModContent> Load()
    {
        foreach (var towerSet in TowerSets)
        {
            foreach (var boostType in BoostTypes)
            {
                yield return new BoostArtifactChange
                {
                    TowerSet = towerSet,
                    BoostType = boostType,
                };
            }
        }
    }

    public override string Description(string description, int index) =>
        Regex.Replace(description, @"(\d+)%",
            match => (int) Math.Round(int.Parse(match.Groups[1].Value) * RogueRemixMod.BoostScale) + "%");

    public override void ModifyArtifact(BoostArtifactModel artifact)
    {
        foreach (var model in artifact.GetDescendants<BoostArtifactBehaviorModel>().ToArray())
        {
            if (model.Is<ProjectileSpeedBoostBehaviorModel>())
            {
                model.multiplier *= RogueRemixMod.BoostScale;
            }
            else
            {
                model.multiplier = 1 + (model.multiplier - 1) * RogueRemixMod.BoostScale;
            }
        }
    }
}