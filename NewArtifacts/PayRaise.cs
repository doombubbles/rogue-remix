using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.DailyChallenge;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;

namespace RogueRemix.NewArtifacts;

public class PayRaise : ModMapArtifact
{
    private static int Amount(int tier) => tier + 1;

    public override string Description(int tier) =>
        $"Gain {Amount(tier)} additional token{(Amount(tier) == 1 ? "" : "s")} whenever you defeat a Bloon Encounter";

    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(MapArtifactModel artifactModel)
    {
    }

    private static int tokenBonus;

    [HarmonyPatch(typeof(RogueMapScreen), nameof(RogueMapScreen.ClaimLoot))]
    internal static class RogueMapScreen_ClaimLoot
    {
        [HarmonyPrefix]
        internal static void Prefix(RogueMapScreen __instance)
        {
            tokenBonus = 0;
            if (__instance.RogueSaveData.rogueLootData.isTokenLoot)
            {
                foreach (var artifact in __instance.RogueSaveData.artifactsInventory)
                {
                    if (artifact.IsArtifact<PayRaise>())
                    {
                        tokenBonus += Amount(artifact.tier);
                    }
                }
            }
            for (var i = 0; i < tokenBonus; i++)
            {
                __instance.RogueSaveData.AddArtifactToInventory(new ArtifactLoot
                {
                    baseId = "Token",
                    artifactName = "Token",
                    lootType = RogueLootType.permanent
                }, true);
            }
        }

        [HarmonyPostfix]
        internal static void Postfix(RogueMapScreen __instance)
        {
            tokenBonus = 0;
        }
    }

    [HarmonyPatch(typeof(PopupScreen), nameof(PopupScreen.ShowRogueRewardPopup))]
    internal static class PopupScreen_ShowRogueRewardPopup
    {
        [HarmonyPrefix]
        internal static void Prefix(PopupScreen __instance, ref int stackCount)
        {
            stackCount += tokenBonus;
        }
    }

}