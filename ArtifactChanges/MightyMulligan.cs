using System;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;

namespace RogueRemix.ArtifactChanges;

public class MightyMulligan : ModVanillaArtifact
{
    public override string Description(string description, int tier) => description + ". Refreshes at start of stage";

    public override string MetaDescription => "Instead of being removed, is refreshed each stage";

    public override void ModifyArtifact(MapArtifactModel artifact)
    {
        artifact.RemoveBehavior<RemoveArtifactAfterEndOfGameRerollBehaviorModel>();
    }

    [HarmonyPatch(typeof(LegendsManager), nameof(LegendsManager.IncreaseRogueStage))]
    internal static class LegendsManager_IncreaseRogueStage
    {
        [HarmonyPostfix]
        internal static void Postfix(LegendsManager __instance)
        {
            foreach (var artifactLoot in __instance.RogueSaveData.artifactsInventory)
            {
                try
                {
                    var artifact = GameData.Instance.artifactsData
                        .GetArtifactData(artifactLoot.artifactName)
                        .ArtifactModel()
                        .Cast<ArtifactModelBase>();
                    artifact.GetDescendants<AddEndOfRoundRerollBehaviorModel>().ForEach(model =>
                    {
                        __instance.RogueSaveData.endOfGameRerolls += model.addedRerolls;
                    });
                }
                catch (Exception e)
                {
                    ModHelper.Warning<RogueRemixMod>(e);
                }

            }
        }
    }
}