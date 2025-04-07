using System.Collections.Generic;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models.Artifacts;

namespace RogueRemix;

public abstract class ModVanillaArtifact : NamedModContent
{
    private static Dictionary<string, ModVanillaArtifact> Cache = new();

    /// <summary>
    /// Tier for Common Artifacts
    /// </summary>
    protected const int Common = 0;

    /// <summary>
    /// Tier for Rare Artifacts
    /// </summary>
    protected const int Rare = 1;

    /// <summary>
    /// Tier for Legendary Artifacts
    /// </summary>
    protected const int Legendary = 2;

    public virtual string BaseId => Name;

    public virtual string? Description1 => null;
    public virtual string? Description2 => null;
    public virtual string? Description3 => null;
    public virtual string? DescriptionX => null;

    public new virtual string? Description(string description, int tier) => tier switch
    {
        0 => Description1,
        1 => Description2,
        2 => Description3,
        _ => DescriptionX
    };

    public new virtual string? DisplayName(string name) => null;

    public sealed override void Register()
    {
        var artifactsData = GameData.Instance.artifactsData;

        var count = 0;
        foreach (var artifact in artifactsData.artifactDatas.Values())
        {
            var artifactModel = artifact.ArtifactModel().Cast<ArtifactModelBase>();
            if (artifactModel.ArtifactName.StartsWith(BaseId))
            {
                ModifyArtifact(artifactModel);
                count++;
            }
        }
        if (count == 0)
        {
            ModHelper.Error<RogueRemixMod>($"No artifacts with baseId {BaseId}");
        }

        Cache[BaseId] = this;
    }

    public void ModifyArtifact(ArtifactModelBase artifact)
    {
        if (artifact.Is(out ItemArtifactModel itemArtifact))
        {
            ModifyArtifact(itemArtifact);
        }
        else if (artifact.Is(out MapArtifactModel mapArtifact))
        {
            ModifyArtifact(mapArtifact);
        }
        else if (artifact.Is(out BoostArtifactModel boostArtifact))
        {
            ModifyArtifact(boostArtifact);
        }
    }

    public virtual void ModifyArtifact(ItemArtifactModel artifact)
    {

    }

    public virtual void ModifyArtifact(MapArtifactModel artifact)
    {

    }

    public virtual void ModifyArtifact(BoostArtifactModel artifact)
    {

    }


    [HarmonyPatch(typeof(ArtifactModelBase), nameof(ArtifactModelBase.GetTitle))]
    internal static class ArtifactModelBase_GetTitle
    {
        [HarmonyPostfix]
        internal static void Postfix(ArtifactModelBase __instance, ref string __result)
        {
            if (Cache.TryGetValue(__instance.baseId, out var modVanillaArtifact) &&
                modVanillaArtifact.DisplayName(__result) is { } displayName)
            {
                __result = displayName;
            }
        }
    }

    [HarmonyPatch(typeof(ArtifactModelBase), nameof(ArtifactModelBase.GetDescription))]
    internal static class ArtifactModelBase_GetDescription
    {
        [HarmonyPostfix]
        internal static void Postfix(ArtifactModelBase __instance, ref string __result)
        {
            if (Cache.TryGetValue(__instance.baseId, out var modVanillaArtifact) &&
                modVanillaArtifact.Description(__result, __instance.tier) is { } description)
            {
                __result = description;
            }
        }
    }
}