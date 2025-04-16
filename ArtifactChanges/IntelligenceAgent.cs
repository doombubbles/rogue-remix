using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace RogueRemix.ArtifactChanges;

public class IntelligenceAgent : ModVanillaArtifact
{
    public override string Description1 =>
        "Heroes give Monkey Intelligence Bureau buff to nearby Monkeys (can hit all Bloon types) but these monkeys have 30% reduced range";
    public override string Description2 =>
        "Heroes give Monkey Intelligence Bureau buff to nearby Monkeys (can hit all Bloon types) but these monkeys have 15% reduced range";
    public override string Description3 =>
        "Heroes give Monkey Intelligence Bureau buff to nearby Monkeys (can hit all Bloon types)";

    public override string MetaDescription => "Attack speed downside is now a range downside";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        var addBehaviors = artifact.GetDescendant<AddTowerBehaviorsArtifactModel>();
        addBehaviors.behaviorModels[1] = new RangeSupportModel("", true,
            -.3f + artifact.tier * .15f, 0, "IntelligenceAgentDebuff", null, false, "", "");

        addBehaviors.dontAddIfScriptsExists = new Il2CppStringArray(0);
    }
}