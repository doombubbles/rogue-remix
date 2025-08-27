using System;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class CrystalBall : ModItemArtifact
{
    public override string DescriptionCommon =>
        "Wizard Monkeys attack 15% faster and have long range targeting within the ranges of other towers";
    public override string DescriptionRare =>
        "Wizard Monkeys attack 20% faster and have long range targeting within the ranges of other towers";
    public override string DescriptionLegendary =>
        "Wizard Monkeys attack 30% faster and have long range targeting within the ranges of other towers";

    public override string InstaMonkey(int tier) => TowerType.WizardMonkey;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Legendary => [2, 0, 0],
        Rare => [2, 1, 0],
        _ => [1, 1, 0]
    };

    public override string Icon => VanillaSprites.DiscoBallIcon; // TODO real icon
    public override bool SmallIcon => true;

    public override TowerSet RarityFrameType => TowerSet.Magic;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new RateBoostBehaviorModel("", artifactModel.tier switch
        {
            Common => .85f,
            Rare => .8f,
            Legendary => .7f,
            _ => 1f,
        }));

    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var model in gameModel.GetTowersWithBaseId(TowerType.WizardMonkey).AsIEnumerable())
        {
            foreach (var attackModel in model.GetAttackModels())
            {
                if (attackModel.GetBehavior<TargetFirstPrioCamoModel>() != null)
                {
                    attackModel.RemoveBehavior<TargetFirstPrioCamoModel>();
                    attackModel.AddBehavior(new TargetFirstSharedRangeModel("",
                        true, true, false, false));
                }

                if (attackModel.GetBehavior<TargetLastPrioCamoModel>() != null)
                {
                    attackModel.RemoveBehavior<TargetLastPrioCamoModel>();
                    attackModel.AddBehavior(new TargetLastSharedRangeModel("",
                        true, true, false, false));
                }

                if (attackModel.GetBehavior<TargetClosePrioCamoModel>() != null)
                {
                    attackModel.RemoveBehavior<TargetClosePrioCamoModel>();
                    attackModel.AddBehavior(new TargetCloseSharedRangeModel("",
                        true, true, false, false));
                }

                if (attackModel.GetBehavior<TargetStrongPrioCamoModel>() != null)
                {
                    attackModel.RemoveBehavior<TargetStrongPrioCamoModel>();
                    attackModel.AddBehavior(new TargetStrongSharedRangeModel("",
                        true, true, false, false));
                }

                if (attackModel.HasBehavior(out AttackFilterModel attackFilterModel))
                {
                    var invisIndex = attackFilterModel.filters
                        .FindIndex(filterModel => filterModel.Is<FilterInvisibleModel>());
                    if (invisIndex >= 0)
                    {
                        var oldFilter = attackFilterModel.filters[invisIndex]!.Cast<FilterInvisibleModel>();
                        attackFilterModel.RemoveChildDependant(attackFilterModel.filters[invisIndex]);
                        var newFilter = attackFilterModel.filters[invisIndex] =
                            new FilterInvisibleSubIntelModel("", oldFilter.isActive, oldFilter.ignoreBroadPhase);
                        attackFilterModel.AddChildDependant(newFilter);
                    }
                }
            }

            model.UpdateTargetProviders();

            foreach (var weaponModel in model.GetWeapons())
            {
                weaponModel.emission.AddBehavior(new EmissionCamoIfTargetIsCamoModel(""));
            }

            var guidedMagic = model.GetWeapon().projectile.GetBehavior<TrackTargetModel>();
            foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToList())
            {
                var travelStraitModel = projectileModel.GetBehavior<TravelStraitModel>();
                if (travelStraitModel != null)
                {
                    var newLifeSpan = travelStraitModel.Lifespan * (200 / travelStraitModel.Speed);
                    travelStraitModel.Lifespan = Math.Max(travelStraitModel.Lifespan, newLifeSpan);
                    if (guidedMagic != null && projectileModel.GetBehavior<TrackTargetModel>() == null)
                    {
                        projectileModel.AddBehavior(guidedMagic.Duplicate());
                    }
                }
            }
        }
    }
}