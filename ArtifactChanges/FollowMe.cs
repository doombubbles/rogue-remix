﻿using System.Linq;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Artifacts.Behaviors;
namespace RogueRemix.ArtifactChanges;

public class FollowMe : ModVanillaArtifact
{
    public override string Description(string description, int index) => description.Split(".").First();

    public override void ModifyArtifact(ItemArtifactModel artifact)
    {
        artifact.GetDescendant<RateBoostBehaviorModel>().multiplier = 1f;
    }
}