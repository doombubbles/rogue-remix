using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class ShinobisStudio : ModItemArtifact
{
    public override string DisplayName => "Shinobi's Studio";

    public override string Description(int tier) => "All Ninjas provide a stack of Shinobi Tactics to each other" +
                                                    (tier > Common
                                                        ? $". Ninja Monkey instas have a {tier} turn shorter cooldown"
                                                        : "");

    public override string Icon => VanillaSprites.ShinobiTacticsUpgradeIcon;
    public override bool SmallIcon => true;
    public override TowerSet RarityFrameType => TowerSet.Magic;

    public override string InstaMonkey(int tier) => TowerType.NinjaMonkey;

    public override int[] InstaTiers(int tier) => tier switch
    {
        Legendary => [0, 0, 2],
        Rare => [0, 1, 2],
        _ => [0, 1, 1],
    };

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        var shinobiBuff = Game.instance.model.GetTower(TowerType.NinjaMonkey, 0, 3, 0)
            .GetDescendant<SupportShinobiTacticsModel>().Duplicate(Name);

        artifactModel.AddTowerBehavior(shinobiBuff, boost =>
        {
            boost.towerTypes = new Il2CppStringArray([TowerType.NinjaMonkey]);
        });
    }

    [HarmonyPatch(typeof(TowerPurchaseButtonRogue), nameof(TowerPurchaseButtonRogue.SetMaxCooldown))]
    internal static class TowerPurchaseButtonRogue_SetMaxCooldown
    {
        [HarmonyPostfix]
        internal static void Postfix(TowerPurchaseButtonRogue __instance)
        {
            if (__instance.rogueInsta.baseId != TowerType.NinjaMonkey) return;

            foreach (var artifact in InGame.Bridge.Simulation.artifactManager.GetActiveArtifacts())
            {
                if (artifact.IsArtifact<ShinobisStudio>())
                {
                    __instance.rogueInsta.currentCooldown -= artifact.artifactBaseModel.tier;
                }
            }
            if (__instance.rogueInsta.currentCooldown < 0)
            {
                __instance.rogueInsta.currentCooldown = 0;
            }
        }
    }
}