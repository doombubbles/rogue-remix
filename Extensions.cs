using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Simulation.Artifacts;

namespace RogueRemix;

public static class Extensions
{
    public static bool IsArtifact<T>(this RogueLoot loot) where T : ModArtifact =>
        loot.Is(out ArtifactLoot artifactLoot) && artifactLoot.baseId == ModContent.GetInstance<T>().Id;

    public static bool IsArtifact<T>(this ArtifactModelBase model) where T : ModArtifact =>
        model.baseId == ModContent.GetInstance<T>().Id;

    public static bool IsArtifact<T>(this ArtifactBase artifactBase) where T : ModArtifact =>
        artifactBase.artifactBaseModel.IsArtifact<T>();

    public static bool HasArtifact<T>(this RogueGameSaveData saveData, int tier = -1) where T : ModArtifact => saveData
        .artifactsInventory
        .Any(loot => loot.baseId == ModContent.GetInstance<T>().Id && (tier == -1 || loot.tier == tier));

    public static bool HasArtifact<T>(this RogueGameSaveData saveData, out int tier) where T : ModArtifact
    {
        var artifact = saveData.artifactsInventory
            .FirstOrDefault(loot => loot.baseId == ModContent.GetInstance<T>().Id);

        tier = artifact?.tier ?? -1;
        return artifact != null;
    }

    public static void OverrideText(this NK_TextMeshProUGUI textMeshProUGUI, string text)
    {
        if (textMeshProUGUI.AutoLocalize)
        {
            textMeshProUGUI.localizeKey = text;
            textMeshProUGUI.UpdateText(text.Localize());
        }
        else
        {
            textMeshProUGUI.UpdateText(text);
        }
    }

    public static string ToOrdinal(this int number) =>
        number +
        (number % 100 is >= 11 and <= 13
            ? "th"
            : (number % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            });


}