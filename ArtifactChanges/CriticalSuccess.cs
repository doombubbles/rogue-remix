using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;

namespace RogueRemix.ArtifactChanges;

public class CriticalSuccess : ModVanillaArtifact
{
    public override string Description(string description, int index) => description.Replace("75%", "50%");

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateBoostBehaviorModel>().multiplier = 1.5f;
    }
}