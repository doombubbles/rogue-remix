using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class TitForTat : ModVanillaArtifact
{
    public override string Description(string description, int index) => description.Replace("move 25%", "move 15%");

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<SlowBloonsZoneModel>().speedScale = 1.15f;
    }
}