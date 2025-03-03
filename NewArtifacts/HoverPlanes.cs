using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class HoverPlanes : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .1f,
        Rare => .15f,
        Legendary => .25f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Monkey Aces deal {Effect(tier):P0} more damage and now fly like Heli Pilots. The Centered Path upgrade unlocks Pursuit mode";

    public override string Icon => VanillaSprites.BiggerJetsUpgradeIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Military;

    public override string InstaMonkey(int tier) => TowerType.MonkeyAce;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [1, 0, 1],
        Rare => [1, 0, 2],
        Legendary => [0, 0, 2],
        _ => []
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new DamageBoostBehaviorModel("", 1 + Effect(artifactModel.tier)),
            boost => boost.towerTypes = new Il2CppStringArray([TowerType.MonkeyAce]));
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var heli = gameModel.GetTower(TowerType.HeliPilot);
        var pursuitHeli = gameModel.GetTower(TowerType.HeliPilot, 2, 0, 0);

        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.MonkeyAce))
        {
            var airUnit = towerModel.GetBehavior<AirUnitModel>();
            var attack = towerModel.GetBehavior<AttackAirUnitModel>();

            var heliMovement = heli.GetDescendant<HeliMovementModel>().Duplicate();
            heliMovement.maxSpeed = airUnit.GetBehavior<PathMovementModel>().speed;

            airUnit.RemoveBehavior<PathMovementModel>();
            airUnit.AddBehavior(heliMovement);

            var shouldHavePursuit = attack.HasBehavior<CenterElipsePatternModel>();
            attack.RemoveBehaviors<TargetSupplierModel>();

            var copyFromAttack = (shouldHavePursuit ? pursuitHeli : heli).GetAttackModel();

            foreach (var targetSupplierModel in copyFromAttack.GetBehaviors<TargetSupplierModel>())
            {
                attack.AddBehavior(targetSupplierModel.Duplicate());
            }

            attack.AddBehavior(copyFromAttack.GetBehavior<RotateToTargetAirUnitModel>().Duplicate());

            foreach (var attackModel in towerModel.GetAttackModels())
            {
                attackModel.fireWithoutTarget = false;
                attackModel.range = 99999;

                foreach (var weaponModel in attackModel.weapons)
                {
                    weaponModel.fireWithoutTarget = false;
                }
            }
        }
    }
}