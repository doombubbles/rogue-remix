using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
namespace RogueRemix.NewArtifacts;

public class ArtifactExpansion : ModBoostArtifact
{
    public override int MinTier => Rare;

    public override string Description(int tier) => $"Artifact Limit increased by {tier}";

    public override string Icon => VanillaSprites.ArtifactPowerIcon;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(BoostArtifactModel artifactModel)
    {
    }
}