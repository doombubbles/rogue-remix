using System.Linq;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class FollowMe : ModVanillaArtifact
{
    public override string Description(string description, int tier) => description.Split(".").First();

    public override string? MetaDescription => "Attack speed downside removed";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateBoostBehaviorModel>().multiplier = 1f;
    }
}