using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.UI_New;

namespace RogueRemix.NewArtifacts;

public class DoubleCash : ModItemArtifact
{
    public override int MinTier => Legendary;

    public override string DescriptionLegendary =>
        "Starting cash and cash per pop is doubled. All tower attack reload time increased 50%";

    public override string Icon => VanillaSprites.DoubleCashIconSmall;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
        artifactModel.AddBoostBehavior(new RateBoostBehaviorModel("", 1.5f));
    }

    public override void OnActivated(Simulation simulation, int tier)
    {
        simulation.model.doubleCashAllowed = true;
        simulation.WasDoubleCashUsed = true;
        foreach (var cashManager in simulation.CashManagers.Values())
        {
            cashManager.doubleCash = true;
            if (!simulation.gameStarted)
            {
                cashManager.cash.Add(cashManager.cash.Value);
            }
        }
        MainHudLeftAlign.instance.OnCashManagersChanged(simulation.CashManagers);
    }
}