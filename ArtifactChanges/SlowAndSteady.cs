using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class SlowAndSteady : ModVanillaArtifact
{
    public override string Description1 =>
        "Monkey damage increased 100%. For the first 20s of each round, all Monkeys attack 1.75x slower";
    public override string Description2 =>
        "Monkey damage increased 100%. For the first 20s of each round, all Monkeys attack 1.65x slower";
    public override string Description3 =>
        "Monkey damage increased 100%. For the first 20s of each round, all Monkeys attack 1.5x slower";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<StartOfRoundRateBuffModel>().modifier = artifact.tier switch
        {
            Common => 1.75f,
            Rare => 1.65f,
            Legendary => 1.5f,
            _ => 1.5f
        };
    }
}