using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppSystem.IO;
namespace RogueRemix.NewArtifacts;

public class TonsOfDamage : ModItemArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .1f,
        Rare => .15f,
        Legendary => .25f,
        _ => 0
    };

    public override string Description(int tier) =>
        $"Most artifacts that boost tower attack speed now instead boost tower damage. All towers do {Effect(tier):P0} more damage";

    public override string Icon => VanillaSprites.MapBuffIconDamage;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new DamageBoostBehaviorModel("", 1 + Effect(artifactModel.tier)));
    }

    [HarmonyPatch(typeof(RateSupportModel.RateSupportMutator), nameof(RateSupportModel.RateSupportMutator.Mutate))]
    internal static class RateSupportMutator_Mutate
    {
        [HarmonyPrefix]
        internal static bool Prefix(RateSupportModel.RateSupportMutator __instance, Model model, ref bool __result)
        {
            if (__instance.id == "ArtifactRateBoost" &&
                InGame.instance?.GameType == GameType.Rogue &&
                RogueLegendsManager.instance.RogueSaveData.HasArtifact<TonsOfDamage>())
            {
                __result = true;

                foreach (var damageModel in model.GetDescendants<DamageModel>().ToArray())
                {
                    if (damageModel.damage > 0)
                    {
                        damageModel.damage = Math.Max(1, damageModel.damage / __instance.multiplier);
                    }
                }

                foreach (var freezeModel in model.GetDescendants<FreezeModel>().ToArray())
                {
                    freezeModel.layers = (int) (freezeModel.layers / __instance.multiplier);
                }

                RulesChanges.ApplyDamageToModifiers(model, 1 / __instance.multiplier);

                return false;
            }

            return true;
        }
    }
}