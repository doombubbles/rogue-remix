using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppSystem.IO;

namespace RogueRemix.NewArtifacts;

public class NowYouSeeMe : ModItemArtifact
{
    public override string DescriptionCommon =>
        "Grants Camo detection to all Towers and Heroes that cannot naturally attack Camo Bloons before Tier 5 / Level 20.";
    public override string DescriptionRare =>
        "Grants Camo detection and 2x Camo damage to all Towers and Heroes that cannot naturally attack Camo Bloons before Tier 5 / Level 20.";
    public override string DescriptionLegendary =>
        "Grants Camo detection and 4x Camo damage to all Towers and Heroes that cannot naturally attack Camo Bloons before Tier 5 / Level 20.";

    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        var baseIds = new List<string>();

        foreach (var (baseId, towerModels) in Game.instance.model.towers
                     .Where(model => !model.IsSubEntity)
                     .GroupBy(model => model.baseId))
        {
            var maxTier = towerModels.Where(model => model.tier < (model.IsHero() ? 20 : 5)).Max(model => model.tier);

            var filterInvisibles = towerModels
                .Where(model => model.tier == maxTier)
                .SelectMany(model => model.GetAttackModels())
                .SelectMany(model => model.GetDescendants<ProjectileModel>().ToArray())
                .SelectMany(model => model.GetDescendants<FilterInvisibleModel>().ToArray())
                .ToArray();

            var camo = filterInvisibles.Length == 0 || filterInvisibles.Any(model => !model.isActive);

            if (!camo)
            {
                baseIds.Add(baseId);
            }

        }

        artifactModel.AddTowerBehavior(new OverrideCamoDetectionModel("", true),
            model => model.towerTypes = baseIds.ToArray());
        artifactModel.AddProjectileBehavior(new DamageModifierForTagModel("", BloonTag.Camo, artifactModel.tier * 2, 0,
            false, false), model => model.towerTypes = baseIds.ToArray());
    }
}