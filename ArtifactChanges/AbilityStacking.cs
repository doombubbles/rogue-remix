using System.Linq;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class AbilityStacking : ModVanillaArtifact
{
    public override string Description(string description, int tier) => description.Split(".").First();

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<CooldownBoostBehaviorModel>().multiplier = 1f;
    }
}