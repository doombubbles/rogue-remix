using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;

namespace RogueRemix.NewArtifacts;

public class AreWeThereYet : ModItemArtifact
{
    public override string Description(int tier) =>
        $"Insta Monkey cooldowns are {tier + 1} turn{(tier != 0 ? "s" : "")} shorter";

    public override string Icon => VanillaSprites.StopWatch;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    [HarmonyPatch(typeof(TowerPurchaseButtonRogue), nameof(TowerPurchaseButtonRogue.SetMaxCooldown))]
    internal static class TowerPurchaseButtonRogue_SetMaxCooldown
    {
        [HarmonyPostfix]
        internal static void Postfix(TowerPurchaseButtonRogue __instance)
        {
            foreach (var artifact in InGame.Bridge.Simulation.artifactManager.GetActiveArtifacts())
            {
                if (artifact.IsArtifact<AreWeThereYet>())
                {
                    __instance.rogueInsta.currentCooldown -= artifact.artifactBaseModel.tier + 1;
                }
            }

            if (__instance.rogueInsta.currentCooldown < 0)
            {
                __instance.rogueInsta.currentCooldown = 0;
            }
        }
    }
}