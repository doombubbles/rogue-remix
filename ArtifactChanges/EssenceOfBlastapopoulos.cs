using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace RogueRemix.ArtifactChanges;

public class EssenceOfBlastapopoulos : ModVanillaArtifact
{
    public override string Description(string description, int tier) =>
        description.Replace("Tier 5 Monkeys and level 20 Heroes", "Heroes and Tier 5 Monkeys").Replace("+4", "+2");

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendants<AddProjectileBehaviorsArtifactModel>().ForEach(model =>
        {
            if (model.towerSets.Contains(TowerSet.Hero))
            {
                model.tiers = new Il2CppStructArray<int>(0);
            }
            model.GetDescendants<AddBonusDamagePerHitToBloonModel>().ForEach(bloonModel =>
            {
                bloonModel.perHitDamageAddition = 2;
            });
        });
    }
}