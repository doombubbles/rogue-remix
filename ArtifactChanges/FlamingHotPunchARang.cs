using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.ArtifactChanges;

public class FlamingHotPunchARang : ModVanillaArtifact
{
    public override string DisplayName(string name) => name.Replace("-A-Rang", "");

    public override string Description(string description, int tier) =>
        description.Replace("Red Hot Rangs", "Red Hot Rangs, White Hot Spikes, Hot Shots, and Heat-tipped Darts");

    public override string MetaDescription => "Renamed, applies also to White Hot Spikes, Hot Shots, and Heat-tipped Darts";

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.RemoveBehavior<InvokeBoostBuffBehaviorModel>();

        var spikes = artifact.GetBehavior<AddProjectileBehaviorsArtifactModel>().Duplicate();
        spikes.towerTypes = new Il2CppStringArray([TowerType.SpikeFactory]);
        spikes.tiers = new Il2CppStructArray<int>([2, 6, 6]);
        artifact.AddBehavior(spikes);

        var tacks = artifact.GetBehavior<AddProjectileBehaviorsArtifactModel>().Duplicate();
        tacks.towerTypes = new Il2CppStringArray([TowerType.TackShooter]);
        tacks.tiers = new Il2CppStructArray<int>([3, 6, 6]);
        artifact.AddBehavior(tacks);

        var darts = artifact.GetBehavior<AddProjectileBehaviorsArtifactModel>().Duplicate();
        darts.towerTypes = new Il2CppStringArray([TowerType.MonkeySub, TowerType.MonkeyBuccaneer]);
        darts.tiers = new Il2CppStructArray<int>([6, 2, 6]);
        artifact.AddBehavior(darts);
    }
}