using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;

namespace RogueRemix.ArtifactChanges;

public class HeightenedPerception : ModVanillaArtifact
{
    public override string Description(string description, int index) => description.Replace("50%", "25%");

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<ProjectileSpeedBoostBehaviorModel>().multiplier = -.25f;
    }
}