using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
namespace RogueRemix.ArtifactChanges;

public class HomingProjectiles : ModVanillaArtifact
{
    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        var towerTypes = artifact.GetDescendant<AddProjectileBehaviorsArtifactModel>().towerTypes;
        var typo = towerTypes.IndexOf("TackShoooter");
        if (typo > -1)
        {
            towerTypes[typo] = TowerType.TackShooter;
        }
    }
}