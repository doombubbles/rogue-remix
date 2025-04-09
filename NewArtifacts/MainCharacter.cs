using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class MainCharacter : ModItemArtifact
{
    private static float Effect(int tier) => .2f - tier * .1f;

    public override string Description(int tier) =>
        "Heroes now count as Primary, Military, Magic, and Support towers." +
        tier switch
        {
            Legendary => "",
            _ => $" Hero attack range reduced by {Effect(tier):P0}"
        };

    public override string Icon => VanillaSprites.HeroesIcon;
    // public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBehavior(new CountAllCategoriesBehaviorModel("",
            new Il2CppStructArray<TowerSet>([TowerSet.Hero]),
            new Il2CppStructArray<TowerSet>([TowerSet.Primary, TowerSet.Military, TowerSet.Magic, TowerSet.Support])));

        artifactModel.AddBoostBehavior(new RangeBoostBehaviorModel("", 1 - Effect(artifactModel.tier)), boost =>
            boost.towerSets = new Il2CppStructArray<TowerSet>([TowerSet.Hero]));
    }

    /// <summary>
    /// Fix interaction with Battlefield Commission where it checks with == TowerSet.Hero
    /// </summary>
    [HarmonyPatch(typeof(HeroXpPerBloonLayerBehavior), nameof(HeroXpPerBloonLayerBehavior.OnBloonDegrade))]
    internal static class HeroXpPerBloonLayerBehavior_OnBloonDegrade
    {
        [HarmonyPrefix]
        internal static void Prefix(HeroXpPerBloonLayerBehavior __instance, Tower poppedBy, ref TowerSet __state)
        {
            __state = TowerSet.None;
            if (poppedBy is {towerModel: not null} && poppedBy.towerModel.towerSet.ContainsFlag(TowerSet.Hero))
            {
                __state = poppedBy.towerModel.towerSet;
                poppedBy.towerModel.towerSet = TowerSet.Hero;
            }
        }

        [HarmonyPostfix]
        internal static void Postfix(HeroXpPerBloonLayerBehavior __instance, Tower poppedBy, ref TowerSet __state)
        {
            if (poppedBy is {towerModel: not null} && __state.ContainsFlag(TowerSet.Hero))
            {
                poppedBy.towerModel.towerSet = __state;
            }
        }
    }
}