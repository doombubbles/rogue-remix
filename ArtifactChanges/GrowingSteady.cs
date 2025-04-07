using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class GrowingSteady : ModVanillaArtifact
{
    public override string Description(string description, int tier) => description
        .Replace("Druids attack 17% slower, but e", "E");

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateBoostBehaviorModel>().multiplier = 1f;
    }
}