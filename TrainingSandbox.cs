using HarmonyLib;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace RogueRemix;

public static class TrainingSandbox
{
    private static bool training;

    // TODO fix training sandbox mode

    /*[HarmonyPatch(typeof(LegendsMainMenuScreen), nameof(LegendsMainMenuScreen.PlayTrainingModeClicked))]
    internal static class LegendsMainMenuScreen_PlayTrainingModeClicked
    {
        [HarmonyPrefix]
        internal static void Prefix(LegendsMainMenuScreen __instance)
        {
            training = true;
        }

        [HarmonyPostfix]
        internal static void Postfix(LegendsMainMenuScreen __instance)
        {
            training = false;
        }
    }

    [HarmonyPatch(typeof(LegendsMainMenuScreen), nameof(LegendsMainMenuScreen.ContinueTrainingMode))]
    internal static class LegendsMainMenuScreen_ContinueTrainingMode
    {
        [HarmonyPrefix]
        internal static void Prefix(LegendsMainMenuScreen __instance)
        {
            training = true;
        }

        [HarmonyPostfix]
        internal static void Postfix(LegendsMainMenuScreen __instance)
        {
            training = false;
        }
    }*/

    [HarmonyPatch(typeof(InGameData), nameof(InGameData.SetupRogueGame))]
    internal static class InGameData_SetupRogueGame
    {
        [HarmonyPostfix]
        internal static void Postfix(InGameData __instance, DailyChallengeModel dcm)
        {
            if (!training || !RogueRemixMod.TrainingSandboxMode) return;

            __instance.selectedMode = "Sandbox";

            training = false;
        }
    }
}