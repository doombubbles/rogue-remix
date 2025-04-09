using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;

namespace RogueRemix.NewArtifacts;

public class AreWeThereYet : ModItemArtifact
{
    public override string Description(int tier) =>
        $"All Monkey placement cooldowns are reduced {tier + 1} round{(tier == Common ? "" : "s")}";

    public override string Icon => VanillaSprites.StopWatch;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        foreach (var tower in TowerType.towers)
        {
            artifactModel.AddBehavior(new InstaCooldownBehaviorModel("", tower, -(artifactModel.tier + 1)));
        }
    }
}