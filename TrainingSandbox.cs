using HarmonyLib;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace RogueRemix;

public static class TrainingSandbox
{
    private static bool training;

    [HarmonyPatch(typeof(RogueNewGameScreen), nameof(RogueNewGameScreen.PlayTrainingModeClicked))]
    internal static class RogueNewGameScreen_PlayTrainingModeClicked
    {
        [HarmonyPrefix]
        internal static void Prefix()
        {
            training = true;
        }

        [HarmonyPostfix]
        internal static void Postfix()
        {
            training = false;
        }
    }

    [HarmonyPatch(typeof(RogueNewGameScreen), nameof(RogueNewGameScreen.ContinueTrainingMode))]
    internal static class RogueNewGameScreen_ContinueTrainingMode
    {
        [HarmonyPrefix]
        internal static void Prefix()
        {
            training = true;
        }

        [HarmonyPostfix]
        internal static void Postfix()
        {
            training = false;
        }
    }

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