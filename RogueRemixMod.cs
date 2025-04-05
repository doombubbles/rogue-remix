using System;
using System.Linq;
using MelonLoader;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using RogueRemix;

[assembly: MelonInfo(typeof(RogueRemixMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace RogueRemix;

public class RogueRemixMod : BloonsTD6Mod
{
    public static readonly ModSettingBool DisableCritPopups = new(true)
    {
        description =
            "Disables Critical Hit popups added by Artifacts, which can noticeably improve performance under certain conditions.",
        requiresRestart = true
    };

    public static readonly ModSettingBool SellingReplacesDiscarding = new(true)
    {
        description = "Discarding an artifact will give tokens the same way selling it would"
    };

    public static readonly ModSettingBool BoostsInShop = new(true)
    {
        description =
            "Makes boost popups no longer happen during matches, and instead Boosts can be purchased at Shop Tiles",
        requiresRestart = true
    };

    public static readonly ModSettingBool ArtifactUpgrading = new(true)
    {
        description =
            "Allows artifacts you already have to appear as loot, and having two of the same artifact of the same tier will upgrade it to one of the higher tier."
    };

    public static readonly ModSettingBool BloonEncounterRewardsRemix = new(false)
    {
        description = "Makes all Bloon Encounters give a reward choice and not just tokens only"
    };

    public static readonly ModSettingBool SkippingRemix = new(false)
    {
        description = "Makes skipping only cost lives if you have an active attempt going on the tile"
    };

    public static readonly ModSettingBool TrackHeroLoadoutCompletions = new(true)
    {
        description = "Saves info for displaying the completionist loadout borders. " +
                      "This is stored in an innocuous spot within your BTD6 profile data, " +
                      "but if you don't want any data stored you can disable this."
    };

    public static readonly ModSettingButton ClearHeroCompletionData = new(() =>
    {
        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Clear Data",
            "This will clear your completion history from your profile and can't be easily undone.", new Action(
                () =>
                {
                    Game.Player.Data.LegendsData.savedLegendsStats.Remove(nameof(RogueRemix));
                    Game.Player.Save();
                }), "OK", null, "Cancel",
            Popup.TransitionAnim.Scale));
    })
    {
        description = "Use this to remove all hero completion data from your BTD6 profile."
    };

    public static readonly ModSettingBool TrainingSandboxMode = new(false)
    {
        description = "Makes the Rogue Legends training mode be Sandbox"
    };

    public override bool UsesArtifactDependants => true;

    public override void OnTitleScreen()
    {
        var data = GameData.Instance.rogueData;
#if DEBUG
        var mapTemplates = data.mapTemplates;
        data.mapTemplates = null;
        FileIOHelper.SaveObject("rogueData.json", data);
        data.mapTemplates = mapTemplates;
#endif

        RulesChanges.ModifyRules(data);

        if (DisableCritPopups)
        {
            foreach (var artifact in GameData.Instance.artifactsData.artifactDatas.Values()
                         .Select(artifact => artifact.ArtifactModel().Cast<ArtifactModelBase>()))
            {
                if (artifact.HasDescendant(out GiveCritChanceBoostBehaviorModel critChanceBoostBehaviorModel))
                {
                    critChanceBoostBehaviorModel.critTextPrefab = new PrefabReference("");
                }
            }
        }
    }

    public override void OnNewGameModel(GameModel gameModel)
    {
        if (InGameData.CurrentGame.rogueData == null || LegendsManager.instance?.RogueSaveData == null) return;

        var artifactsInventory = LegendsManager.instance.RogueSaveData.artifactsInventory;
        var artifacts = GameData.Instance.artifactsData.artifactDatas
            .Values()
            .Select(artifactDateBase => artifactDateBase.ArtifactModel().Cast<ArtifactModelBase>())
            .ToList();

        foreach (var modArtifact in ModContent.GetContent<ModArtifact>())
        {
            if (modArtifact is not IArtifactSynergy artifactSynergy) continue;

            var tier = artifactsInventory.FirstOrDefault(loot => loot.baseId.Contains(modArtifact.Id))?.tier ?? -1;

            artifactSynergy.ModifyOtherArtifacts(artifacts, tier);
        }
    }
}