namespace RogueRemix.ArtifactChanges;

public class DreamTeams : ModVanillaArtifact
{
    public override string? Description(string description, int index) =>
        description.Replace("have faster", "have 10% faster");
}