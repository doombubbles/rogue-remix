using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class SwellingSpikes : ModVanillaArtifact
{
    public override string Description1 =>
        "Spike Factories attack half as fast, but have 130% more pierce. Adds a 1-0-1 Spike Factory to your Party";
    public override string Description2 =>
        "Spike Factories attack half as fast, but have 140% more pierce. Adds a 2-0-1 Spike Factory to your Party";
    public override string Description3 =>
        "Spike Factories attack half as fast, but have 170% more pierce. Adds a 2-0-0 Spike Factory to your Party";

    public override string MetaDescription => "Pierce buff increased";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        var boost = artifact.GetDescendant<RateBoostBehaviorModel>();
        boost.multiplier = 2 + (boost.multiplier - 2) * 2;
    }
}