using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapUnits.Models;

namespace HexMapUnits.Tests;

internal class TestUtils
{
    private static readonly UnitBase _exampleUnit = new()
    {
        Id = 0,
        Position = new CubeCoordinates(0, 0, 0),
        Layer = 0,
        Player = 1,
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

    // Clone all properties (deep copy mutable collections)
    private static UnitBase CloneUnit(UnitBase unit)
    {
        if (unit == null) return null!;
        return new UnitBase
        {
            Id = unit.Id,
            Player = unit.Player,
            Health = unit.Health,
            MaxHealth = unit.MaxHealth,
            Name = unit.Name,
            Images = new List<string>(unit.Images),
            Description = unit.Description,
            Type = unit.Type,
            Era = unit.Era,
            MaxMovement = unit.MaxMovement,
            MovementType = unit.MovementType,
            Movement = unit.Movement,
            WeaponType = unit.WeaponType,
            CombatStrength = unit.CombatStrength,
            RangedAttack = unit.RangedAttack,
            Range = unit.Range,
            Fortification = unit.Fortification,
            Seed = unit.Seed,
            Sight = unit.Sight,
            CanAttack = unit.CanAttack,
            CanBuildCity = unit.CanBuildCity,
            Goods = new Dictionary<int, int>(unit.Goods),
            ProductionCost = unit.ProductionCost,
            PurchaseCost = unit.PurchaseCost,
            UpkeepCost = unit.UpkeepCost,
            Position = unit.Position,
            Layer = unit.Layer
        };
    }

    internal static UnitBase GetExampleUnit()
    {
        return CloneUnit(_exampleUnit);
    }
}
