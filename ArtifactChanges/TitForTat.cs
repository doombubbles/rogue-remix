using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class TitForTat : ModVanillaArtifact
{
    public override string Description(string description, int tier) => description.Replace("move 25%", "move 17%");

    public override string? MetaDescription => "Bloon speed downside decreased";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<SlowBloonsZoneModel>().speedScale = 1.17f;
    }
}