#if DEBUG

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using DiffMatchPatch;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppNinjaKiwi.Common;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using RogueRemix.ArtifactChanges;
using UnityEngine;

namespace RogueRemix;

public static class MarkdownGeneration
{
    private static string SourcePath => Path.Combine(ModHelper.ModSourcesDirectory, nameof(RogueRemix));
    private static string NewArtifactsPath => Path.Combine(SourcePath, "NewArtifacts.md");
    private static string ArtifactChangesPath => Path.Combine(SourcePath, "ArtifactChanges.md");
    private static string ImagesFolder => Path.Combine(SourcePath, "Images");

    public static IEnumerator Generate()
    {
        var newBoosts =
            """
            <h1>New Boosts</h1>

            | Boost | Common | Rare | Legendary |
            |----------|--------|------|-----------|

            """;

        var newArtifacts =
            """

            <h1>New Artifacts</h1>

            | Artifact | Common | Rare | Legendary |
            |----------|--------|------|-----------|

            """;

        foreach (var modArtifact in ModContent.GetInstance<RogueRemixMod>().Content.OfType<ModArtifact>())
        {

            var boost = modArtifact is ModBoostArtifact;
            var cells = new[] {"", "—", "—", "—"};

            cells[0] =
                $"""<a href="/NewArtifacts/{modArtifact.Name}.cs"> <p align="center" ><img src="Images/{modArtifact.Name}.png" width=50 /> <br/> {modArtifact.DisplayName} </p></a>""";

            var icon = ResourceLoader.LoadAsync(modArtifact.IconCommonReference.Cast<IAssetReference<Sprite>>());

            yield return icon;

            icon.Result.TrySaveToPNG(Path.Combine(ImagesFolder, modArtifact.Name + ".png"));

            foreach (var (tier, index) in modArtifact.Tiers)
            {
                cells[tier + 1] = LocalizationManager.Instance.GetText(modArtifact.GetId(index) + "Description");
            }

            var row = "| " + cells.Join(delimiter: " | ") + " |\n";

            if (boost)
            {
                newBoosts += row;
            }
            else
            {
                newArtifacts += row;
            }

            ModHelper.Msg<RogueRemixMod>($"Generated {modArtifact.DisplayName}");
        }

        File.WriteAllText(NewArtifactsPath, newArtifacts + newBoosts);


        var artifactChanges =
            """

            <h1>Artifact Changes</h1>

            | Artifact | Common | Rare | Legendary |
            |----------|--------|------|-----------|

            """;

        foreach (var modVanillaArtifact in ModContent.GetInstance<RogueRemixMod>().Content.OfType<ModVanillaArtifact>())
        {
            var boost = false; // modVanillaArtifact is BoostArtifactChange;
            var cells = new[] {"", "—", "—", "—"};

            var models = GameData.Instance.artifactsData.artifactDatas.Values()
                .Select(data => data.ArtifactModel().Cast<ArtifactModelBase>())
                .Where(model => model.ArtifactName.StartsWith(modVanillaArtifact.BaseId))
                .OrderBy(model => model.tier)
                .ToList();

            var ogName = LocalizationManager.Instance.textTable[models.First().nameLocKey]
                .Replace(" Common", "").Replace(" Rare", "").Replace(" Legendary", "").Replace(" {0}", "");
            var name = LocalizationManager.Instance.GetText(models.First().nameLocKey)
                .Replace(" Common", "").Replace(" Rare", "").Replace(" Legendary", "").Replace(" {0}", "");
            var finalName = GetStrikethroughDiff(ogName, name);

            var icon = ResourceLoader.LoadAsync(models.First().icon.Cast<IAssetReference<Sprite>>());

            yield return icon;

            icon.Result.TrySaveToPNG(Path.Combine(ImagesFolder, modVanillaArtifact.Name + ".png"));

            var i = 0;
            var count = 0;
            foreach (var model in models)
            {
                var ogDescription = model.GetDescription(LocalizationManager.Instance.Cast<ILocProvider>());
                var newDescription = modVanillaArtifact.Description(ogDescription, i++);

                if (newDescription is not null)
                {
                    cells[model.tier + 1] = GetStrikethroughDiff(ogDescription, newDescription);
                    count++;
                }
            }

            if (count == 0)
            {
                continue;
            }

            cells[0] =
                $"""<a href="/ArtifactChanges/{(boost ? "BoostArtifactChange" : modVanillaArtifact.Name)}.cs"> <p align="center" ><img src="Images/{modVanillaArtifact.Name}.png" width=50 /> <br/> {finalName} </p></a>""";


            var row = "| " + cells.Join(delimiter: " | ") + " |\n";

            artifactChanges += row;

            ModHelper.Msg<RogueRemixMod>($"Generated {name}");
        }


        File.WriteAllText(ArtifactChangesPath, artifactChanges);
    }

    private static string GetStrikethroughDiff(string oldText, string newText)
    {
        var dmp = new diff_match_patch();

        var a = dmp.diff_linesToWords(oldText.Replace(", ", " , ").Replace(". ", " . "),
            newText.Replace(", ", " , ").Replace(". ", " . "));
        var lineText1 = (string) a[0];
        var lineText2 = (string) a[1];
        var lineArray = (IList<string>) a[2];

        var diffs = dmp.diff_main(lineText1, lineText2, false);
        dmp.diff_cleanupSemantic(diffs);

        dmp.diff_charsToLines(diffs, lineArray);


        var result = new System.Text.StringBuilder();

        foreach (var diff in diffs)
        {
            switch (diff.operation)
            {
                case Operation.DELETE:
                    result.Append($" ~~{diff.text.Trim()}~~ ");
                    break;
                case Operation.INSERT:
                    result.Append($" **{diff.text.Trim()}** ");
                    break;
                case Operation.EQUAL:
                    result.Append(diff.text);
                    break;
            }
        }

        return result.ToString().RegexReplace(@"\s+\.", ".").RegexReplace(@"\s+,", ",");
    }

}

#endif