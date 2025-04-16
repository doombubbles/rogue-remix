using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace RogueRemix.ArtifactChanges;

public class AlchemicEngineering : ModVanillaArtifact
{
    public override string Description1 =>
        "Tier 3 and higher Engineers convert Bloons to Gold. Adds a 0-1-1 Engineer to your Party";
    public override string Description2 =>
        "Tier 3 and higher Engineers have pierce increased 15% and convert Bloons to Gold. Adds a 0-1-2 Engineer to your Party";
    public override string Description3 =>
        "Tier 3 and higher Engineers have pierce increased 45% and convert Bloons to Gold. Adds a 0-0-2 Engineer to your Party";

    public override string MetaDescription => "Pierce downsides decreased";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        if (!artifact.HasDescendant(out PierceBoostBehaviorModel pierceBoostBehaviorModel))
        {
            pierceBoostBehaviorModel = new PierceBoostBehaviorModel("", 1f);
            artifact.AddBoostBehavior(pierceBoostBehaviorModel, boost =>
            {
                boost.towerTypes = new Il2CppStringArray([TowerType.EngineerMonkey]);
                boost.tiers = new Il2CppStructArray<int>([3, 3, 3]);
            });
        }

        pierceBoostBehaviorModel.multiplier = artifact.tier switch
        {
            Common => 1f,
            Rare => 1.15f,
            Legendary => 1.45f,
            _ => 1f
        };
    }
}