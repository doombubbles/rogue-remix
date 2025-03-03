using System.Linq;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace RogueRemix.ArtifactChanges;

public class TheUnspokenHeroes : ModVanillaArtifact
{
    public override string Description(string description, int index) => description.Split(".").First();

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateBoostBehaviorModel>().multiplier = 1f;
        artifact.GetDescendant<HeroEndOfRoundXpScaleBehaviorModel>().scale = 1f;
        // artifact.GetDescendant<GiveCritChanceBoostBehaviorModel>().critTextPrefab = new PrefabReference("");
    }
}