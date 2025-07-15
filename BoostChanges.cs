using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Simulation.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppSystem.Linq;

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

    /// <summary>
    /// Fix Precision artifacts making Tack Shooters not 360 degrees
    /// </summary>
    [HarmonyPatch(typeof(SpreadBoostBehavior.SpreadBoostMutator),
        nameof(SpreadBoostBehavior.SpreadBoostMutator.Mutate))]
    internal static class SpreadBoostMutator_Mutate
    {
        [HarmonyPrefix]
        internal static void Prefix(Model model, ref List<ArcEmissionModel> __state)
        {
            __state = model.GetDescendants<ArcEmissionModel>()
                .ToArray()
                .Where(emissionModel => emissionModel.angle >= 360f)
                .ToList();
        }

        [HarmonyPostfix]
        internal static void Postfix(Model model, ref List<ArcEmissionModel> __state)
        {
            foreach (var arcEmissionModel in __state)
            {
                arcEmissionModel.angle = 360f;
            }
        }
    }

    /// <summary>
    /// Fix Essence of Phayze not working for paragons
    /// </summary>
    [HarmonyPatch(typeof(OverrideCamoDetection), nameof(OverrideCamoDetection.FirstUpdate))]
    internal static class OverrideCamoDetection_FirstUpdate
    {
        [HarmonyPrefix]
        internal static void Prefix(OverrideCamoDetection __instance)
        {
            if (__instance.tower.IsParagonBased())
            {
                __instance.mutator.isArtifactMutator = true;
            }
        }
    }
}