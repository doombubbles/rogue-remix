using System.Collections.Generic;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class ItsATrap : ModMapArtifact
{
    public override string Icon => VanillaSprites.CamoTrapIcon;
    public override bool SmallIcon => true;

    public override string Description(int tier) => $"Grants {tier + 1} Camo Trap and Glue Trap per tile";

    public override IEnumerable<ModContent> Load() => ModHelper.HasMod("PowersInShop") ? base.Load() : [];

    public override void ModifyArtifactModel(MapArtifactModel artifactModel)
    {
        artifactModel.AddBehavior(new AddRogueBoostBehaviorModel("", new RogueInstaMonkey
        {
            baseId = "PowersInShop-CamoTrap",
            tiers = new Il2CppStructArray<int>(0)
        }, artifactModel.tier + 1));
        artifactModel.AddBehavior(new AddRogueBoostBehaviorModel("", new RogueInstaMonkey
        {
            baseId = "PowersInShop-GlueTrap",
            tiers = new Il2CppStructArray<int>(0)
        }, artifactModel.tier + 1));
    }
}