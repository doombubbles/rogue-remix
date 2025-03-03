using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;

namespace RogueRemix.ArtifactChanges;

public class BBGatlingGun : ModVanillaArtifact
{
    public override string Description1 =>
        "Main attack damage of Monkeys is reduced to 90%, but attack reload time is reduced 15%";
    public override string Description2 =>
        "Main attack damage of Monkeys is reduced to 85%, but attack reload time is reduced 25%";
    public override string Description3 =>
        "Main attack damage of Monkeys is reduced to 80%, but attack reload time is reduced 40%";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<DamageBoostBehaviorModel>().multiplier = artifact.tier switch
        {
            Common => .9f,
            Rare => .85f,
            Legendary => .8f,
            _ => .75f
        };
    }
}