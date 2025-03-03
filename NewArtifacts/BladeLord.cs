using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
namespace RogueRemix.NewArtifacts;

public class BladeLord : ModItemArtifact
{
    private static float Interval(int tier) => tier switch
    {
        Common => .5f,
        Rare => .4f,
        Legendary => .3f,
        _ => 1
    };

    public override string Description(int tier) =>
        $"Blade Shooters are permanently orbited by three of their projectiles that damage Bloons every {Interval(tier):N1} seconds";

    public override string Icon => VanillaSprites.BladeMaelstromUpgradeIcon;
    public override bool SmallIcon => true;

    public override string InstaMonkey(int tier) => TowerType.TackShooter;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [0, 1, 1],
        Rare => [0, 2, 1],
        Legendary => [0, 2, 0],
        _ => []
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var orbitAttack = gameModel.GetTower(TowerType.BoomerangMonkey, 5, 0, 0).GetAttackModel("Orbit").Duplicate();
        var orbitProj = orbitAttack.GetDescendant<ProjectileModel>();
        orbitProj.RemoveBehavior<DamageModifierModel>();
        orbitProj.RemoveBehavior<DamageModel>();

        foreach (var towerModel in gameModel.towers.Where(model =>
                     model.baseId == TowerType.TackShooter && model.tiers[1] >= 3))
        {
            var weapon = towerModel.GetAttackModel().weapons[0]!;
            var projectile = weapon.projectile.Duplicate();

            if (weapon.emission.Is(out ArcEmissionModel arcEmissionModel))
            {
                projectile.pierce *= arcEmissionModel.count;
            }

            projectile.AddFilter(new FilterAllModel(""));
            projectile.RemoveBehavior<TravelStraitModel>();
            projectile.AddBehavior(new AgeModel("", 9999999, 9999999, true, null));
            projectile.AddBehavior(new DontDestroyOnContinueModel(""));
            projectile.AddBehavior(new CantBeReflectedModel(""));

            towerModel.AddBehavior(new OrbitModel("", projectile, 3, towerModel.range * .75f));

            var newAttack = orbitAttack.Duplicate();
            var newWeapon = newAttack.weapons[0]!;
            var newProj = newWeapon.projectile;

            newWeapon.Rate = Interval(tier);

            newProj.AddBehavior(projectile.GetDamageModel().Duplicate());
            foreach (var damageModifierForTagModel in projectile.GetBehaviors<DamageModifierForTagModel>())
            {
                newProj.AddBehavior(damageModifierForTagModel.Duplicate());
            }
            newProj.radius = 10 + towerModel.range * .75f;

            towerModel.AddBehavior(newAttack);

        }
    }
}