using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Artifacts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppSystem.IO;
namespace RogueRemix.NewArtifacts;

public class TheMoreTheMerrier : ModItemArtifact
{
    private const int Amount = 2;

    private static int Interval(int tier) => tier switch
    {
        Common => 6,
        Rare => 5,
        Legendary => 3,
        _ => 1
    };

    public override string Description(int tier) =>
        $"Towers that emit a spread of two or more projectiles per attack have doubled projectile count on every {Interval(tier).ToOrdinal()} attack";

    public override string Icon => VanillaSprites.OverdriveUpgradeIcon;
    public override bool SmallIcon => true;

    public override void ModifyArtifactModel(ItemArtifactModel artifactModel)
    {
    }

    public override void ModifyGameModel(GameModel gameModel, int tier)
    {
        foreach (var tower in gameModel.towers)
        {
            foreach (var weapon in tower.GetDescendants<WeaponModel>().ToArray())
            {
                EmissionModel newEmission;
                if (weapon.emission.Is(out ArcEmissionModel arcEmissionModel) && arcEmissionModel.Count >= Amount)
                {
                    var clone = arcEmissionModel.Duplicate();
                    clone.Count *= 2;
                    newEmission = clone;
                }
                else if (weapon.emission.Is(out RandomEmissionModel randomEmissionModel) &&
                         randomEmissionModel.count >= Amount)
                {
                    var clone = randomEmissionModel.Duplicate();
                    clone.count *= 2;
                    newEmission = clone;
                }
                else if (weapon.emission.Is(out ParallelEmissionModel parallelEmissionModel) &&
                         parallelEmissionModel.count >= Amount)
                {
                    var clone = parallelEmissionModel.Duplicate();
                    clone.count *= 2;
                    newEmission = clone;
                }
                else if (weapon.emission.Is(out AlternatingArcEmissionModel alternatingArcEmissionModel) &&
                         alternatingArcEmissionModel.count >= Amount)
                {
                    var clone = alternatingArcEmissionModel.Duplicate();
                    clone.count *= 2;
                    newEmission = clone;
                }
                else if (weapon.emission.Is(out EmissionWithOffsetsModel emissionWithOffsetsModel) &&
                         emissionWithOffsetsModel.projectileCount >= Amount)
                {
                    var clone = emissionWithOffsetsModel.Duplicate();
                    clone.projectileCount *= 2;
                    newEmission = clone;
                }
                else if (weapon.emission.Is(out AdoraEmissionModel adoraEmissionModel) &&
                         adoraEmissionModel.count >= Amount)
                {
                    var clone = adoraEmissionModel.Duplicate();
                    clone.count *= 2;
                    newEmission = clone;
                }
                else continue;

                weapon.AddBehavior(new AlternateProjectileModel("", weapon.projectile, newEmission, Interval(tier)));
            }
        }
    }
}