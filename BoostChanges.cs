using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
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
            if (!RogueRemixMod.BoostsInShop) return;

            __instance.transform.GetComponentFromChildrenByName<NK_TextMeshProUGUI>("TitleLeft").OverrideText("Boosts");
            __instance.transform.GetComponentFromChildrenByName<NK_TextMeshProUGUI>("TitleRight")
                .OverrideText("Artifacts");

            foreach (var artifactIcon in __instance.activeArtifactIcons.ToArray())
            {
                if (artifactIcon.artifactModel.IsBoost)
                {
                    artifactIcon.transform.SetParent(__instance.boostInventoryContainer.transform);
                }
                else if (artifactIcon.artifactModel.baseId == "Token")
                {
                    artifactIcon.gameObject.SetActive(false);
                    __instance.activeArtifactIcons.Remove(artifactIcon);
                }
            }
        }
    }
}