using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation;
using Il2CppSystem.Linq;
using CreateEffectOnExpireModel = Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel;

namespace RogueRemix.NewArtifacts;

public class ExplosiveGrowth : ModItemArtifact, IArtifactSynergy
{
    private static float Effect(int tier) => tier switch
    {
        Legendary => .25f,
        Rare => .15f,
        Common => .1f,
        _ => 0f
    };

    public override string Description(int tier) =>
        $"All Explosions are {Effect(tier) * 2:P0} larger and have {Effect(tier):P0} increased pierce and damage";

    public override string Icon => VanillaSprites.BombBlitzUpgradeIcon;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var happenedOn = new Dictionary<string, HashSet<string>>();
        foreach (var tower in gameModel.towers)
        {
            foreach (var projectileModel in tower.GetDescendants<ProjectileModel>().ToArray())
            {
                ProjectileModel? proj = null;
                EffectModel? effect = null;

                if (projectileModel.HasBehavior(out CreateProjectileOnContactModel onContact) &&
                    IsExplosion(onContact.projectile))
                {
                    proj = onContact.projectile;
                    effect = projectileModel.GetBehavior<CreateEffectOnContactModel>()?.effectModel;
                }
                else if (projectileModel.HasBehavior(out CreateProjectileOnExhaustFractionModel onExhaust) &&
                         IsExplosion(onExhaust.projectile))
                {
                    proj = onExhaust.projectile;
                    effect = projectileModel.GetBehavior<CreateEffectOnExhaustFractionModel>()?.effectModel;
                }
                else if (projectileModel.HasBehavior(out CreateProjectileOnExpireModel onExpire) &&
                         IsExplosion(onExpire.projectile))
                {
                    proj = onExpire.projectile;
                    effect = projectileModel.GetBehavior<CreateEffectOnExpireModel>()?.effectModel;
                }
                else if (projectileModel.HasBehavior(out CreateProjectileOnExhaustPierceModel onPierce) &&
                         IsExplosion(onPierce.projectile))
                {
                    proj = onPierce.projectile;
                }
                else if (projectileModel.id == "MoabExplode")
                {
                    proj = projectileModel;
                    effect = projectileModel.GetBehavior<CreateEffectOnExhaustFractionModel>().effectModel;
                }

                if (proj != null)
                {
                    Modify(proj, effect, tier);
                    happenedOn.TryAdd(tower.baseId, []);

                    happenedOn[tower.baseId].Add(tower.appliedUpgrades.LastOrDefault() ?? "");
                }
            }
        }

        /*foreach (var (tower, upgrades) in happenedOn)
        {
            ModHelper.Msg<RogueRemixMod>($"Happened on {tower} for {upgrades.Join()}");
        }*/
    }

    private static bool IsExplosion(ProjectileModel? projectile) => projectile != null &&
                                                                    projectile.HasBehavior<DamageModel>() &&
                                                                    (projectile.id.Contains("Explosion") ||
                                                                     projectile.id.Contains("Blast"));

    private static void Modify(ProjectileModel proj, EffectModel? effect, int tier)
    {
        proj.GetDamageModel().damage *= 1 + Effect(tier);
        proj.pierce *= 1 + Effect(tier);
        proj.radius *= 1 + Effect(tier);

        if (effect != null)
        {
            effect.scale *= 1 + Effect(tier) * 2;
        }
    }

    public void ModifyOtherArtifacts(List<ArtifactModelBase> artifacts, int tier)
    {
        foreach (var artifact in artifacts)
        {
            if (artifact.baseId == "SplodeyDarts")
            {
                var create = artifact.GetDescendant<TowerCreateProjectileOnProjectileExhaustModel>();
                var proj = create.projectileModel;
                var effect = create.effectModel;
                var damageModel = proj.GetDamageModel();

                proj.radius = IArtifactSynergy.RestoreStore(proj.radius, Name + artifact.ArtifactName + "Radius");
                proj.pierce = IArtifactSynergy.RestoreStore(proj.radius, Name + artifact.ArtifactName + "Pierce");
                damageModel.damage = IArtifactSynergy.RestoreStore(damageModel.damage, Name + artifact.ArtifactName + "Damage");
                effect.scale = IArtifactSynergy.RestoreStore(effect.scale, Name + artifact.ArtifactName + "Scale");

                Modify(proj, effect, tier);
            }

            if (artifact.baseId == "ExplosiveEcononics")
            {
                var create = artifact.GetDescendant<CreateProjectileOnTowerDestroyModel>();
                var proj = create.projectileModel;
                var damageModel = proj.GetDamageModel();

                proj.radius = IArtifactSynergy.RestoreStore(proj.radius, Name + artifact.ArtifactName + "Radius");
                proj.pierce = IArtifactSynergy.RestoreStore(proj.radius, Name + artifact.ArtifactName + "Pierce");
                damageModel.damage = IArtifactSynergy.RestoreStore(damageModel.damage, Name + artifact.ArtifactName + "Damage");
                proj.scale = IArtifactSynergy.RestoreStore(proj.scale, Name + artifact.ArtifactName + "Scale");

                Modify(proj, null, tier);
            }
        }
    }
}