using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
namespace RogueRemix;

public static class BoostChanges
{

    /// <summary>
    /// Rename UI and rearrange boosts
    /// </summary>
    [HarmonyPatch(typeof(RogueArtifactPopup), nameof(RogueArtifactPopup.OpenShowInventory))]
    internal static class RogueArtifactPopup_OpenShowInventory
    {
        [HarmonyPostfix]
        internal static void Postfix(RogueArtifactPopup __instance)
        {
            if (!RogueRemixMod.BoostsRemix) return;

            __instance.transform.GetComponentFromChildrenByName<NK_TextMeshProUGUI>("TitleLeft").OverrideText("Boosts");
            __instance.transform.GetComponentFromChildrenByName<NK_TextMeshProUGUI>("TitleRight").OverrideText("Artifacts");

            foreach (var artifactIcon in __instance.activeArtifactIcons.ToArray())
            {
                if (artifactIcon.artifactModel.IsBoost)
                {
                    artifactIcon.transform.SetParent(__instance.boostInventoryContainer.transform);
                } else if (artifactIcon.artifactModel.baseId == "Token")
                {
                    artifactIcon.gameObject.SetActive(false);
                    __instance.activeArtifactIcons.Remove(artifactIcon);
                }
            }
        }
    }

    /// <summary>
    /// Block in game boost popups
    /// </summary>
    [HarmonyPatch(typeof(PopupScreen), nameof(PopupScreen.ShowRogueLootPopup))]
    internal static class PopupScreen_ShowRogueLootPopup
    {
        [HarmonyPrefix]
        internal static bool Prefix(PopupScreen.ReturnCallback? okCallback,
            PopupScreen.ReturnCallback? onRewardScreenClosedCallback)
        {
            if (RogueRemixMod.BoostsRemix && InGame.instance != null)
            {
                okCallback?.Invoke();
                onRewardScreenClosedCallback?.Invoke();
                return false;
            }

            return true;
        }
    }

}