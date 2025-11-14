#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Commands;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using Random = Il2CppSystem.Random;

namespace RogueRemix;

public class ArtifactCommand : ModCommand
{
    public override string Command => "artifact";
    public override string Help => "deals with artifacts";

    public override bool IsAvailable => RogueLegendsManager.instance?.LegendsData?.selectedRogueSave != null;

    public override bool Execute(ref string resultText) => ExplainSubcommands(out resultText);
}

public class ArtifactTestCommand : ModCommand<ArtifactCommand>
{
    public override string Command => "test";
    public override string Help => "test";

    public override bool Execute(ref string resultText)
    {
        var rand = new Random();
        var legendaryCount = 0;
        var rareCount = 0;
        var commonCount = 0;
        for (var i = 0; i < 1e6; i++)
        {
            var tier = RogueLegendsManager.instance.GetRandomArtifactTier(rand);
            switch (tier)
            {
                case 2:
                    legendaryCount++;
                    break;
                case 1:
                    rareCount++;
                    break;
                default:
                    commonCount++;
                    break;
            }
        }

        resultText = $"Legendary: {legendaryCount / 1e6:P2} Rare: {rareCount / 1e6:P2} Common: {commonCount / 1e6:P2}";
        return true;
    }

}

public class GiveArtifactCommand : ModCommand<ArtifactCommand>
{
    public override string Command => "give";
    public override string Help => "get an artifact";

    protected override float RegistrationPriority => 100;

    public override void Register()
    {
        base.Register();
        foreach (var artifactId in GameData.Instance.artifactsData.artifactDatas.Keys())
        {
            var command = new GiveSpecificArtifactCommand
            {
                ArtifactId = artifactId
            };
            mod.AddContent(command);
            command.Register();
        }
    }

    public override bool Execute(ref string resultText) => ExplainSubcommands(out resultText);
}

public class GiveSpecificArtifactCommand : ModCommand<GiveArtifactCommand>
{
    public string ArtifactId { get; init; } = null!;

    public override string Command => ArtifactId;
    public override string Help => $"Receive the {ArtifactId.Localize()} artifact";

    public override IEnumerable<ModContent> Load() => [];

    [Value(0, Default = 1, Required = false, HelpText = "how many to give (if this is a Token)")]
    public int Amount { get; set; }

    [Value(1, Default = false, Required = false, HelpText = "whether this is a boost and not a permanent")]
    public bool Boost { get; set; }

    public override bool Execute(ref string resultText)
    {
        if (!GameData.Instance.artifactsData.artifactDatas.TryGetValue(ArtifactId, out var artifact))
        {
            resultText = "No artifact data found for this artifact";
            return false;
        }

        var artifactModel = artifact.ArtifactModel().Cast<ArtifactModelBase>();
        var loot = new ArtifactLoot
        {
            artifactName = artifactModel.name,
            tier = artifactModel.tier,
            baseId = artifactModel.baseId,
            lootType = Boost ? RogueLootType.boost : RogueLootType.permanent,
            startingArtifact = false
        };

        try
        {
            for (var i = 0; i < Amount; i++)
            {
                RogueLegendsManager.instance.RogueSaveData.AddArtifactToInventory(loot, true);
            }
            PopupScreen.instance.ShowRogueRewardPopup(new Action(() => { }), loot, false, Amount);

            if (InGame.instance != null)
            {
                InGame.Bridge.Simulation.artifactManager.Activate(artifactModel.ArtifactName);
            }
        }
        catch (Exception e)
        {
            resultText = e.ToString();
            return false;
        }

        return true;
    }

}

public class InstaCommand : ModCommand
{
    public override string Command => "insta";

    public override string Help => "Deals with rogue instas";

    public override bool IsAvailable => RogueLegendsManager.instance?.LegendsData?.selectedRogueSave != null;

    public override bool Execute(ref string resultText) => ExplainSubcommands(out resultText);
}

public class GiveInstaCommand : ModCommand<InstaCommand>
{
    public override string Command => "give";

    public override string Help => "Give an insta";

    public override bool Execute(ref string resultText)
    {
        return false;
    }
}

public class GiveSpecificInstaCommand : ModCommand<GiveInstaCommand>
{
    public string BaseId { get; init; } = null!;

    public override string Command => BaseId;

    public override string Help => $"Get the insta {BaseId.Localize()}";

    [Value(0, Default = 0, Required = false, HelpText = "top tier")]
    public int Top { get; set; }

    [Value(1, Default = 0, Required = false, HelpText = "mid tier")]
    public int Mid { get; set; }

    [Value(2, Default = 0, Required = false, HelpText = "bot tier")]
    public int Bot { get; set; }

    public override IEnumerable<ModContent> Load() => TowerType.towers.Select(s => new GiveSpecificInstaCommand
    {
        BaseId = s,
    });

    public override bool Execute(ref string resultText)
    {
        try
        {
            var insta = new RogueInstaMonkey
            {
                baseId = BaseId,
                lootType = InGame.instance != null ? RogueLootType.boost : RogueLootType.permanent,
                tiers = new Il2CppStructArray<int>([Top, Mid, Bot]),
                currentCooldown = 0,
                uniqueId = RogueLegendsManager.instance.GetNextInstaUniqueId()
            };

            if (InGame.instance != null)
            {
                InGame.Bridge.Simulation.rogueInstaInventory.AddInstaTower(insta);
                ShopMenu.instance.QueueRebuildTowerSet();
            }
            else
            {
                RogueLegendsManager.instance.AddInstaToInventory(insta);
            }
        }
        catch (Exception e)
        {
            resultText = e.ToString();
            return false;
        }

        return true;
    }
}

public class GenerateRogueRemixCommand : ModCommand<GenerateCommand>
{
    public override string Command => "rogueremix";
    public override string Help => "Generates Rogue Remix markdown files";

    public override bool Execute(ref string resultText)
    {
        resultText = "Starting coroutine";
        MelonCoroutines.Start(MarkdownGeneration.Generate());
        return true;
    }
}

#endif