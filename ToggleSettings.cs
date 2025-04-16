using System;
using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppSystem.IO;
using RogueRemix.NewArtifacts;
using UnityEngine;
namespace RogueRemix;

public class ToggleSettings : ModContent, IModSettings
{
    protected override float RegistrationPriority => 10000;

    public static readonly ModSettingCategory NewArtifacts = new("New Artifacts")
    {
        modifyCategory = category =>
        {
            var enableAll = category.AddButton(
                new Info("Enable All", 1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    foreach (var setting in GetInstance<RogueRemixMod>().ModSettings.Values
                                 .Where(setting => setting.category == NewArtifacts)
                                 .OfType<ModSettingBool>())
                    {
                        setting.SetValue(true);
                    }
                }));
            enableAll.LayoutElement.ignoreLayout = true;
            enableAll.AddText(new Info("Text", InfoPreset.FillParent), "Enable All", 80f);

            var disableAll = category.AddButton(
                new Info("Disable All", -1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.RedBtnLong, new Action(() =>
                {
                    foreach (var setting in GetInstance<RogueRemixMod>().ModSettings.Values
                                 .Where(setting => setting.category == NewArtifacts)
                                 .OfType<ModSettingBool>())
                    {
                        setting.SetValue(false);
                    }
                }));
            disableAll.LayoutElement.ignoreLayout = true;
            disableAll.AddText(new Info("Text", InfoPreset.FillParent), "Disable All", 80f);
        }
    };

    public static readonly ModSettingCategory ArtifactChanges = new("Artifact Changes")
    {
        modifyCategory = category =>
        {
            var enableAll = category.AddButton(
                new Info("Enable All", 1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    foreach (var setting in ModContent.GetInstance<RogueRemixMod>().ModSettings.Values
                                 .Where(setting => setting.category == ArtifactChanges)
                                 .OfType<ModSettingBool>())
                    {
                        setting.SetValue(true);
                    }
                }));
            enableAll.LayoutElement.ignoreLayout = true;
            enableAll.AddText(new Info("Text", InfoPreset.FillParent), "Enable All", 80f);

            var disableAll = category.AddButton(
                new Info("Disable All", -1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.RedBtnLong, new Action(() =>
                {
                    foreach (var setting in ModContent.GetInstance<RogueRemixMod>().ModSettings.Values
                                 .Where(setting => setting.category == ArtifactChanges)
                                 .OfType<ModSettingBool>())
                    {
                        setting.SetValue(false);
                    }
                }));
            disableAll.LayoutElement.ignoreLayout = true;
            disableAll.AddText(new Info("Text", InfoPreset.FillParent), "Disable All", 80f);
        }
    };


    public override IEnumerable<ModContent> Load()
    {
        foreach (var type in typeof(RogueRemixMod).Assembly.DefinedTypes)
        {
            if (type.IsAbstract) continue;

            if (type.IsAssignableTo(typeof(ModArtifact)) || type.IsAssignableTo(typeof(ModVanillaArtifact)))
            {
                var setting = new ModSettingBool(true)
                {
                    button = true,
                };
                if (type.IsAssignableTo(typeof(ModArtifact)))
                {
                    setting.category = NewArtifacts;
                    setting.enabledText = "Active";
                    setting.disabledText = "Inactive";
                }
                if (type.IsAssignableTo(typeof(ModVanillaArtifact)))
                {
                    setting.category = ArtifactChanges;
                    setting.enabledText = "Changed";
                    setting.disabledText = "Unchanged";
                }
                mod.ModSettings.Add(type.Name, setting);
            }
        }

        return base.Load();
    }

    internal static readonly Dictionary<string, ModSettingBool> Cache = new();

    public override void Register()
    {
        foreach (var modArtifact in mod.Content.OfType<ModArtifact>())
        {
            var setting = (ModSettingBool) mod.ModSettings[modArtifact.GetType().Name];
            setting.icon = modArtifact.IconCommonReference.AssetGUID;
            setting.displayName = modArtifact.DisplayName;
            setting.description = "Controls whether this modded artifact will show up in loot";

            foreach (var (tier, _) in modArtifact.Tiers)
            {
                setting.description += $"\n{tier switch {
                    0 => "Common", 1 => "Rare", 2 => "Legendary", _ => "?"
                }}: {modArtifact.Description(tier)}";
            }

            foreach (var id in modArtifact.Ids)
            {
                Cache[id] = setting;
            }
        }

        foreach (var modVanillaArtifact in mod.Content.OfType<ModVanillaArtifact>())
        {
            var setting = (ModSettingBool) mod.ModSettings[modVanillaArtifact.GetType().Name];
            var artifacts = GameData.Instance.artifactsData.GetBaseArtifactModels(modVanillaArtifact.BaseId).ToArray();
            setting.icon = artifacts.First().icon.AssetGUID;
            setting.requiresRestart = true;
            setting.displayName = modVanillaArtifact.BaseDisplayName;
            setting.description = "Controls whether this vanilla artifact is altered:\n" +
                                  modVanillaArtifact.MetaDescription;
        }

        foreach (var (key, value) in mod.ModSettings)
        {
            Localize(mod, key + " Setting Name", value.displayName);
            Localize(mod, key + " Setting Description", value.description);
        }
    }

    [HarmonyPatch(typeof(LegendsManager), nameof(LegendsManager.HasArtifactInLootPoolAlready))]
    internal static class LegendsManager_HasArtifactInLootPoolAlready
    {
        [HarmonyPrefix]
        internal static bool Prefix(LegendsManager __instance, ArtifactModelBase artifactModel, ref bool __result)
        {
            if (!RogueRemixMod.BoostsInShop && artifactModel.baseId.Contains(nameof(Unboosted)) ||
                Cache.TryGetValue(artifactModel.ArtifactName, out var setting) && !setting)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}