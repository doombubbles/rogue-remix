using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Models.Gameplay.Mods;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RogueRemix.NewArtifacts;

public class DoubleCash : ModItemArtifact
{
    public override int MinTier => Legendary;

    public override string DescriptionLegendary =>
        "Starting cash and cash per pop is doubled. All towers attack 50% slower";

    public override string Icon => VanillaSprites.DoubleCashIconSmall;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new RateBoostBehaviorModel("", 1.5f));
        artifactModel.monkeyKnowledgeModModels = new ModModel("",
            new Il2CppReferenceArray<MutatorModModel>([new StartingCashModModel("", 0, 0, 2)]));
    }

    public override void OnActivated(Simulation simulation, int tier)
    {
        simulation.model.doubleCashAllowed = true;
        simulation.WasDoubleCashUsed = true;
        foreach (var cashManager in simulation.CashManagers.Values())
        {
            cashManager.doubleCash = true;
        }
        MainHudLeftAlign.instance.OnCashManagersChanged(simulation.CashManagers);
    }
}