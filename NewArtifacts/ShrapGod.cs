using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;

namespace RogueRemix.NewArtifacts;

public class ShrapGod : ModItemArtifact
{
    private static int Amount(int tier) => tier switch
    {
        Common => 1,
        Rare => 2,
        Legendary => 3,
        _ => 0
    };


    public override string Description(int tier) =>
        $"Shrapnel Shot bullets create {Amount(tier)} additional shrapnel piece{(Amount(tier) == 1 ? "" : "s")} that also damage{(Amount(tier) == 1 ? "s" : "")} the Bloon that was shot";

    public override string Icon => VanillaSprites.ShrapnelShotUpgradeIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Military;

    public override string InstaMonkey(int tier) => TowerType.SniperMonkey;

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
        foreach (var tower in gameModel.GetTowersWithBaseId(TowerType.SniperMonkey)
                     .Where(model => model.appliedUpgrades.Contains(UpgradeType.ShrapnelShot)))
        {
            var mainProj = tower.GetAttackModel().weapons[0]!.projectile;
            var emitOnDamage = mainProj.GetBehavior<EmitOnDamageModel>();

            var emission = emitOnDamage.emission.Duplicate().Cast<ArcEmissionModel>();
            emission.Count = Amount(tier);
            emission.angle = emission.Count * .075f;

            var proj = emitOnDamage.projectile.Duplicate();
            proj.SetDisplay("d3d9d6b931126d443a8bfc160b610d7c");

            mainProj.AddBehavior(new CreateProjectileOnContactModel("", proj,
                emission, false, false, false));
        }
    }
}