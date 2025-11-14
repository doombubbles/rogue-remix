using System.Linq;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;

namespace RogueRemix.NewArtifacts;

public class TargetAcquired : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .15f,
        Rare => .2f,
        Legendary => .35f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"All Dartling Gunners and Mortar Monkeys have First / Last / Close / Strong targeting options and {Effect(tier):P0} more pierce";

    public override string Icon => VanillaSprites.TargetSetIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Military;

    public override string InstaMonkey(int tier) => tier == Rare ? TowerType.MortarMonkey : TowerType.DartlingGunner;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [1, 1, 0],
        Rare => [0, 2, 0],
        Legendary => [0, 2, 0],
        _ => []
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var dartling = true;
        var mortar = true;

        if (ModHelper.HasMod("TacticalTweaks", out var tacticalTweaks))
        {
            if (tacticalTweaks.ModSettings.TryGetValue("DartlingGunnerTargeting", out var dartlingGunnerTargeting) &&
                dartlingGunnerTargeting is ModSettingBool dartlingEnabled)
            {
                dartling = !dartlingEnabled;
            }

            if (tacticalTweaks.ModSettings.TryGetValue("MortarMonkeyTargeting", out var mortarMonkeyTargeting) &&
                mortarMonkeyTargeting is ModSettingBool mortarEnabled)
            {
                mortar = !mortarEnabled;
            }
        }

        if (dartling)
        {
            foreach (var model in gameModel.towers.Where(model =>
                         model.baseId == TowerType.DartlingGunner &&
                         !model.appliedUpgrades.Contains(UpgradeType.BloonAreaDenialSystem)))
            {
                var attackModel = model.GetAttackModel();

                UpdatePointer(attackModel);
                AddAllTargets(attackModel);

                model.UpdateTargetProviders();
            }
        }

        if (mortar)
        {
            foreach (var model in gameModel.towers.Where(model => model.baseId == TowerType.MortarMonkey))
            {
                var attackModel = model.GetAttackModel();

                AddAllTargets(attackModel);

                model.towerSelectionMenuThemeId = "ActionButton";
                model.UpdateTargetProviders();
            }
        }
    }


    public static void AddAllTargets(AttackModel attackModel)
    {
        var prevTargets = attackModel.GetBehaviors<TargetSupplierModel>().ToList();

        attackModel.AddBehavior(new TargetFirstModel("", true, false));
        attackModel.AddBehavior(new TargetLastModel("", true, false));
        attackModel.AddBehavior(new TargetCloseModel("", true, false));
        attackModel.AddBehavior(new TargetStrongModel("", true, false));

        foreach (var target in prevTargets)
        {
            attackModel.RemoveBehavior(target);
            attackModel.AddBehavior(target);
        }
    }

    public static void UpdatePointer(AttackModel attackModel)
    {
        var pointer = attackModel.GetBehavior<RotateToPointerModel>();
        attackModel.AddBehavior(new RotateToTargetModel("", false, false, pointer.rotateOnlyOnEmit, 0,
            pointer.rotateTower, false));

        if (attackModel.HasDescendant(out LineEffectModel lineEffectModel))
        {
            lineEffectModel.useRotateToPointer = false;
        }
    }
}