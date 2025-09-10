using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.SimulationBehaviors;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace RogueRemix.NewArtifacts;

public class GiveMeFive : ModItemArtifact
{
    public static readonly float PriceReduction = .5f;

    public override string DescriptionLegendary =>
        $"You can have 1 more of each Tier 5 tower. Tier 5 upgrades and instas cost {PriceReduction:P0} less";

    public override string Icon => VanillaSprites.InstaT5Icon;

    public override int MinTier => Legendary;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        var restrictions = gameModel.towers
            .Where(tower => !tower.IsHero() && tower.tier == 5 && tower.tiers.Sum() == 5 && !tower.IsSubEntity)
            .Select(tower => new TowerTierRestrictionModel(tower.name, tower.baseId, tower.tiers.IndexOf(5), 5, 1))
            .ToArray();

        gameModel.towerTierRestrictions = gameModel.towerTierRestrictions.Concat(restrictions).ToArray();
        gameModel.AddChildDependants(restrictions);

        foreach (var upgrade in gameModel.upgrades.Where(upgrade => upgrade.tier == 4))
        {
            upgrade.cost -= (int) (upgrade.cost * PriceReduction);
        }
    }

    [HarmonyPatch(typeof(RogueLegendsManager), nameof(RogueLegendsManager.GetInstaIngameCost), typeof(string),
        typeof(Il2CppStructArray<int>), typeof(RogueLootType))]
    internal static class RogueLegendsManager_GetInstaIngameCost
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueLegendsManager __instance, Il2CppStructArray<int> tiers, ref float __result)
        {
            if (tiers.Contains(5) && __instance.RogueSaveData.HasArtifact<GiveMeFive>())
            {
                __result = CostHelper.CostForDifficulty((int) __result, 1 - PriceReduction);
            }
        }
    }
}