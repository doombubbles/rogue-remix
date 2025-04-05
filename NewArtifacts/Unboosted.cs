using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
namespace RogueRemix.NewArtifacts;

public class Unboosted : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .03f,
        Rare => .06f,
        Legendary => .1f,
        _ => 0f,
    };

    public override string Description(int tier) =>
        $"If you have no Boosts, all towers' damage, pierce, range, and attack speed is increased by {Effect(tier):P0} per stage";

    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        if (LegendsManager.instance.RogueSaveData.artifactsInventory.Where(loot => loot.lootType == RogueLootType.permanent &&
                loot.baseId != "Token" &&
                loot.artifactName.Contains("BoostArtifact")).Count > 0) return;

        var amount = 1 + Effect(tier) * (LegendsManager.instance.RogueSaveData.stage + 1);

        foreach (var tower in gameModel.towers)
        {
            tower.range *= amount;
            tower.GetDescendants<AttackModel>().ForEach(model => model.range *= amount);
            tower.GetDescendants<WeaponModel>().ForEach(model => model.Rate /= amount);
            tower.GetDescendants<ProjectileModel>().ForEach(model => model.pierce *= amount);
            tower.GetDescendants<DamageModel>().ForEach(model => model.damage *= amount);
        }
    }
}