using System.Linq;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class AbsoluteAbility : ModVanillaArtifact
{
    public override string Description(string description, int tier) => description.Replace("10%", "5%");

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateBoostBehaviorModel>().multiplier = 1f;
    }
}