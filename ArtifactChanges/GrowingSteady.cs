using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class GrowingSteady : ModVanillaArtifact
{
    public override string Description(string description, int index) => description
        .Replace("Druids attack reload time is increased 20%, but every round, ", "");

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateBoostBehaviorModel>().multiplier = 1f;
    }
}