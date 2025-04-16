using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace RogueRemix.ArtifactChanges;

public class EssenceOfDreadbloon : ModVanillaArtifact
{
    public override string Description(string description, int tier) =>
        description.Replace("Tier 5 Monkeys and level 20 Heroes", "Heroes and Tier 5 Monkeys");

    public override string MetaDescription => "Hero level 20 restriction removed";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendants<AddProjectileBehaviorsArtifactModel>().ForEach(model =>
        {
            if (model.towerSets.Contains(TowerSet.Hero))
            {
                model.tiers = new Il2CppStructArray<int>(0);
            }
        });
    }
}