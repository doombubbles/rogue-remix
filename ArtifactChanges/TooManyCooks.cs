using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class TooManyCooks : ModVanillaArtifact
{
    public override string MetaDescription => "Internal stack size limited so it's never net negative";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateSupportModel>().maxStackSize = artifact.tier switch
        {
            Common => 5,
            Rare => 6,
            Legendary => 9,
            _ => artifact.tier,
        };
    }
}