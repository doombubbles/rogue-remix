using System.Linq;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
namespace RogueRemix.NewArtifacts;

public class ArtifactExpansion : ModBoostArtifact
{
    public override int MinTier => Rare;

    public override string Description(int tier) => $"Artifact Limit increased by {tier * .1:P0}";

    public override string Icon => VanillaSprites.ArtifactPowerIcon;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(BoostArtifactModel artifactModel)
    {
    }

    private static int ArtifactCount(RogueGameSaveData saveData) => saveData.artifactsInventory
        .ToArray().Count(loot => !RogueData.ignoreCountArtifacts.Contains(loot.artifactName));

    private static int ArtifactLimit(RogueGameSaveData saveData) =>
        (int) (saveData.modifiers.artifactLimit *
               (1 +
                .1f *
                saveData.artifactsInventory.ToArray()
                    .Where(a => a.baseId.Contains(nameof(ArtifactExpansion))).Sum(a => a.tier)));

    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.ResetUIs))]
    internal static class DisplayArtifactsPanel_ResetUIs
    {
        [HarmonyPostfix]
        internal static void Postfix(DisplayArtifactsPanel __instance)
        {
            if (__instance.artifactCountTxt != null)
            {
                __instance.artifactCountTxt.OverrideText(
                    ArtifactCount(__instance.RogueSaveData) + "/" + ArtifactLimit(__instance.RogueSaveData));
            }
        }
    }

    /// <summary>
    /// Recalculate
    /// </summary>
    [HarmonyPatch(typeof(DisplayArtifactsPanel), nameof(DisplayArtifactsPanel.OpenDelete))]
    internal static class DisplayArtifactsPanel_OpenDelete
    {
        [HarmonyPrefix]
        internal static bool Prefix(DisplayArtifactsPanel __instance)
        {
            return ArtifactCount(__instance.RogueSaveData) > ArtifactLimit(__instance.RogueSaveData);
        }
    }
}