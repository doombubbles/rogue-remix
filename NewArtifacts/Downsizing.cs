using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class Downsizing : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .1f,
        Rare => .15f,
        Legendary => .25f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Towers cost {Effect(tier):P0} less and are {Effect(tier):P0} smaller";

    public override string Icon => VanillaSprites.SmallMonkeysModeIcon;
    public override bool SmallIcon => true;


    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var tower in gameModel.towers)
        {
            tower.cost = CostHelper.CostForDifficulty((int) tower.cost, 1 - Effect(tier));

            if (tower.footprint.Is(out CircleFootprintModel circle))
            {
                circle.radius *= 1 - Effect(tier);
            }
            else if (tower.footprint.Is(out RectangleFootprintModel rectangle))
            {
                rectangle.xWidth *= 1 - Effect(tier);
                rectangle.yWidth *= 1 - Effect(tier);
            }

            tower.displayScale = 1 - Effect(tier);
            tower.radius *= 1 - Effect(tier);
            tower.RadiusSquared *= tower.radius * tower.radius;
        }
    }

    [HarmonyPatch(typeof(RogueLegendsManager), nameof(RogueLegendsManager.GetInstaIngameCost), typeof(string),
        typeof(Il2CppStructArray<int>), typeof(RogueLootType))]
    internal static class RogueLegendsManager_GetInstaIngameCost
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueLegendsManager __instance, ref float __result)
        {
            if (__instance.RogueSaveData.HasArtifact<Downsizing>(out var tier))
            {
                __result = CostHelper.CostForDifficulty((int) __result, 1 - Effect(tier));
            }
        }
    }
}