using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppSystem.IO;
namespace RogueRemix.NewArtifacts;

public class CamoBeGone : ModItemArtifact
{
    private const int PierceAmount = 3;

    private static float Effect(int tier) => tier switch
    {
        Common => .1f,
        Rare => .15f,
        Legendary => .25f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Projectiles that De-Camo Bloons have {PierceAmount}x pierce, and the towers that fire them have {Effect(tier):P0} more attack speed";

    public override string Icon => VanillaSprites.ShimmerUpgradeIcon;

    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var tower in gameModel.towers)
        {
            var modified = false;
            foreach (var proj in tower.GetDescendants<ProjectileModel>().ToArray())
            {
                if (proj.behaviors.Any(b => b.Is(out RemoveBloonModifiersModel remover) && remover.cleanseCamo))
                {
                    proj.pierce *= PierceAmount;
                    modified = true;
                }
            }

            if (modified)
            {
                foreach (var weapon in tower.GetDescendants<WeaponModel>().ToArray())
                {
                    weapon.Rate /= 1 + Effect(tier);
                }
            }
        }
    }
}