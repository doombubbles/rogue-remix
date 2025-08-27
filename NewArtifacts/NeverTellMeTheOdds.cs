using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppSystem.IO;
using Math = Il2CppAssets.Scripts.Simulation.SMath.Math;

namespace RogueRemix.NewArtifacts;

public class NeverTellMeTheOdds : ModItemArtifact, IArtifactSynergy
{
    private static float Effect(int tier) => tier switch
    {
        Common => .5f,
        Rare => 1f,
        Legendary => 2f,
        _ => 0f
    };

    public override string Description(int tier) =>
        $"Critical Hits and other random chance effects for towers are {Effect(tier):P0} more common";

    public override string Icon => VanillaSprites.RandomIcon;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {

    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var towerModel in gameModel.towers)
        {
            Apply(towerModel, tier);
        }
    }

    public static void Apply(Model model, int tier)
    {
        foreach (var crit in model.GetDescendants<CritMultiplierModel>().ToArray())
        {
            crit.lower = Math.Max(1, Math.FloorToInt(crit.lower / (1 + Effect(tier))));
            crit.upper = Math.Max(1, Math.CeilToInt(crit.upper / (1 + Effect(tier))));
        }

        foreach (var crit in model.GetDescendants<GiveCritChanceBoostBehaviorModel>().ToArray())
        {
            crit.lower = Math.Max(1, Math.FloorToInt(crit.lower / (1 + Effect(tier))));
            crit.upper = Math.Max(1, Math.CeilToInt(crit.upper / (1 + Effect(tier))));
        }

        foreach (var wind in model.GetDescendants<WindModel>().ToArray())
        {
            wind.chance = Math.Min(1, wind.chance * (1 + Effect(tier)));
        }

        foreach (var addBehavior in model.GetDescendants<AddBehaviorToBloonModel>().ToArray())
        {
            addBehavior.chance = Math.Min(1, addBehavior.chance * (1 + Effect(tier)));
        }
    }

    public void ModifyOtherArtifacts(List<ArtifactModelBase> artifacts, int tier)
    {
        foreach (var artifact in artifacts)
        {
            if (artifact.HasDescendant(out GiveCritChanceBoostBehaviorModel crit))
            {
                crit.upper = IArtifactSynergy.RestoreStore(crit.upper, Name + artifact.ArtifactName + "Crit");
            }
            else if (artifact.HasDescendant(out WindModel wind))
            {
                wind.chance = IArtifactSynergy.RestoreStore(wind.chance, Name + artifact.ArtifactName + "Wind");
            }
            else if (artifact.HasDescendant(out TowerCreateProjectileOnProjectileExhaustModel createProj))
            {
                createProj.chance = Math.Min(1, createProj.chance * (1 + Effect(tier)));
            }
            else continue;

            Apply(artifact, tier);
        }
    }
}