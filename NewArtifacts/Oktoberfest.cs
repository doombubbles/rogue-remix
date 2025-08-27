using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;
namespace RogueRemix.NewArtifacts;

public class Oktoberfest : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => 1,
        Rare => 2,
        Legendary => 4,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Alchemist buff potions last for {Effect(tier):P0} more shots";

    public override string Icon => VanillaSprites.PermanentBrewUpgradeIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Magic;

    public override string InstaMonkey(int tier) => TowerType.Alchemist;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Common => [1, 1, 0],
        Rare => [2, 1, 0],
        Legendary => [2, 0, 0],
        _ => []
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var model in gameModel.GetTowersWithBaseId(TowerType.Alchemist).AsIEnumerable())
        {
            if (model.HasDescendant(out AddBerserkerBrewToProjectileModel brew))
            {
                brew.cap += (int) (brew.cap * Effect(tier));

                var brewCheck = brew.towerBehaviors[0]!.Cast<BerserkerBrewCheckModel>();
                brewCheck.maxCount += (int) (brewCheck.maxCount * Effect(tier));
            }

            if (model.HasDescendant(out AddAcidicMixtureToProjectileModel dip))
            {
                dip.cap += (int) (dip.cap * Effect(tier));
                var dipCheck = dip.towerBehaviors[0]!.Cast<AcidicMixtureCheckModel>();
                dipCheck.maxCount += (int) (dipCheck.maxCount * Effect(tier));
            }
        }
    }
}