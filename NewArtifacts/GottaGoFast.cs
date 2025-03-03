using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation;

namespace RogueRemix.NewArtifacts;

public class GottaGoFast : ModItemArtifact
{
    public override string Description(int tier) =>
        $"All Monkeys with a \"Faster _____\" upgrade attack an extra {Speed(tier):P0} faster";

    public override string Icon => VanillaSprites.FasterDartsUpgradeIcon;
    public override bool SmallIcon => true;

    private static float Speed(int tier) => tier switch
    {
        Common => .15f,
        Rare => .2f,
        Legendary => .35f,
        _ => 0f,
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var tower in gameModel.towers)
        {
            if (tower.appliedUpgrades.Any(s => s.Contains("Faster")))
            {
                tower.GetDescendants<WeaponModel>().ForEach(model => model.Rate *= 1 - Speed(tier));
            }
        }
    }
}