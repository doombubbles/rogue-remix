using System;
using System.Linq;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class SpiritOfAdventure : ModVanillaArtifact
{
    public override string? Description(string description, int index) => description.Split(".").First();

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendants<RateBoostBehaviorModel>()
            .ForEach(model => model.multiplier = Math.Min(model.multiplier, 1f));
    }
}