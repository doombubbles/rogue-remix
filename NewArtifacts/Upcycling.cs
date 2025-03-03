using System;
using System.Collections.Generic;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;

namespace RogueRemix.NewArtifacts;

public class Upcycling : ModMapArtifact
{
    public override int MinTier => Legendary;

    public override string Description(int tier) =>
        "Whenever you remove an Artifact from your inventory, instead of getting Tokens you receive a random Common Boost";

    public override string Icon => VanillaSprites.FullyBoosted;

    public override IEnumerable<ModContent> Load() => [];

    public override void ModifyArtifactModel(MapArtifactModel artifactModel)
    {
    }

    public static bool Handle(ArtifactModelBase artifactModel)
    {
        if (!LegendsManager.instance.RogueSaveData.HasArtifact<Upcycling>() ||
            artifactModel.IsArtifact<Upcycling>() ||
            artifactModel.IsBoost)
        {
            return false;
        }

        var boost = LegendsManager.instance.GetRandomArtifacts(1, RogueLootType.boost, 0,
            LegendsManager.instance.RogueSaveData.seed +
            (int) (LegendsManager.instance.RogueSaveData.currentPosition?.GetValueOrDefault().magnitude ?? 0) +
            artifactModel.ArtifactName.GetHashCode(), 0).First();
        boost.lootType = RogueLootType.permanent;

        LegendsManager.instance.RogueSaveData.AddArtifactToInventory(boost, true);
        PopupScreen.instance.ShowRogueRewardPopup(new Action(() => { }), boost, false);

        return true;
    }
}