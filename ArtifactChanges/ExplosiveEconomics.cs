using System.Linq;
using Il2CppAssets.Scripts.Models.Artifacts;
namespace RogueRemix.ArtifactChanges;

public class ExplosiveEconomics : ModVanillaArtifact
{
    public override string Description(string description, int index) => description.Split(".").First();

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        // seems like it already doesn't do that?
    }
}