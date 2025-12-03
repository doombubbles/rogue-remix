using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.DailyChallenge;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using UnityEngine;

namespace RogueRemix;

public static class Animations
{
    [HarmonyPatch(typeof(RogueMonkeyMovement), nameof(RogueMonkeyMovement.UpdateMovement))]
    internal static class RogueMonkeyMovement_Update
    {
        [HarmonyPrefix]
        internal static void Prefix(RogueMonkeyMovement __instance)
        {
            __instance.movementSpeed = 1 / RogueRemixMod.RogueMapAnimationSpeed;
        }

        [HarmonyPostfix]
        internal static void Postfix(RogueMonkeyMovement __instance)
        {
            __instance.movementSpeed = 1 / RogueRemixMod.RogueMapAnimationSpeed;
        }
    }

    [HarmonyPatch(typeof(RogueMap), nameof(RogueMap.Update))]
    internal static class RogueMap_Update
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueMap __instance)
        {
            __instance.tileInteractDelay = 1 / RogueRemixMod.RogueMapAnimationSpeed;
            __instance.bossDeathAnimTime = 7 / RogueRemixMod.RogueMapAnimationSpeed;
            __instance.smallPopupTime = 2 / RogueRemixMod.RogueMapAnimationSpeed;
        }
    }

    [HarmonyPatch(typeof(RogueMapScreen), nameof(RogueMapScreen.Open))]
    internal static class RogueMapScreen_Open
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {
            Time.timeScale = RogueRemixMod.RogueMapAnimationSpeed;
        }
    }

    [HarmonyPatch(typeof(RogueMapScreen), nameof(RogueMapScreen.Close))]
    internal static class RogueMapScreen_Close
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {
            Time.timeScale = 1;
        }
    }
}