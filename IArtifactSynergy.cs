using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.Artifacts;

namespace RogueRemix;

public interface IArtifactSynergy
{
    public void ModifyOtherArtifacts(List<ArtifactModelBase> artifacts, int tier);


    private static readonly Dictionary<string, object> Defaults = [];

    public static T RestoreStore<T>(T currentOrDefault, string id)
    {
        if (Defaults.TryGetValue(id, out var value)) return (T) value;

        Defaults[id] = currentOrDefault!;

        return currentOrDefault;
    }
}