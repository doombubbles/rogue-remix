using System.Collections.Generic;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Simulation;

namespace RogueRemix.NewArtifacts;

public class CashInjection : ModBoostArtifact
{
    private static float Effect(int tier) => tier switch
    {
        Common => .15f,
        Rare => .20f,
        Legendary => .30f,
        _ => 0f,
    };

    public override string Description(int tier) => $"Starting cash increased {Effect(tier):P0}";

    public override string Icon => VanillaSprites.MoneyBag;
    public override bool SmallIcon => true;

    public override IEnumerable<ModContent> Load() => RogueRemixMod.BoostsInShop ? base.Load() : [];

    public override void ModifyArtifactModel(BoostArtifactModel artifactModel)
    {
    }

    public override void OnActivated(Simulation simulation, int tier)
    {
        if (simulation.gameStarted) return;

        foreach (var cashManager in simulation.CashManagers.Values())
        {
            cashManager.cash.Add(cashManager.cash.Value * Effect(tier));
        }
    }
}