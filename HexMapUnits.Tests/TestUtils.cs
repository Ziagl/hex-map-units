using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapUnits.Models;

namespace HexMapUnits.Tests;

internal class TestUtils
{
    private static readonly UnitType _exampleUnit = new()
    {
        Type = 1,
        Health = 100,
        MaxHealth = 100,
        Movement = 10,
        MaxMovement = 10,
        WeaponType = 1,
        CombatStrength = 10,
        RangedAttack = 5,
        Range = 1,
        Sight = 1,
        CanAttack = false,
        CanBuildCity = false,
        Goods = new() { { 1, 2 } },
        ProductionCost = 0,
        PurchaseCost = 0,
        UpkeepCost = 0,
        Seed = 0
    };

    internal static UnitBase GetExampleUnit()
    {
        return UnitFactory.CreateUnitBase(_exampleUnit, 0, new CubeCoordinates(0,0,0), 0);
    }
}
