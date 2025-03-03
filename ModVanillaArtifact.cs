using System.Collections.Generic;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models.Artifacts;

namespace RogueRemix;

public abstract class ModVanillaArtifact : NamedModContent
{
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

    public new virtual string? Description(string description, int index) => index switch
    {
        0 => Description1,
        1 => Description2,
        2 => Description3,
        _ => null
    };

    public override void RegisterText(Il2CppSystem.Collections.Generic.Dictionary<string, string> textTable)
    {
        for (var i = Common; i <= Legendary; i++)
        {
            var key = BaseId + (i + 1) + "Description";
            if (textTable.TryGetValue(key, out var value) && Description(value, i) is { } description)
            {
                var text = new ModVanillaArtifactDescription
                {
                    Key = key,
                    Description = description
                };
                mod.AddContent(text);
                text.Register();
            }
        }
    }

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
}

public class ModVanillaArtifactDescription : ModTextOverride
{
    public string Key { get; init; } = null!;
    public string Description { get; init; } = null!;

    public override string Name => Key;
    public override string LocalizationKey => Key;
    public override bool Active => true;
    public override string TextValue => Description;

    public override IEnumerable<ModContent> Load() => [];
}