using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapUnits.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexMapUnits.Tests;

internal class Utils
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

    private static UnitBase CloneUnit(UnitBase unit)
    {
        return new UnitBase
        {
            Id = unit.Id,
            Position = unit.Position,
            Layer = unit.Layer,
            Player = unit.Player,
            Type = unit.Type,
            Health = unit.Health,
            MaxHealth = unit.MaxHealth,
            Movement = unit.Movement,
            MaxMovement = unit.MaxMovement,
            CombatStrength = unit.CombatStrength,
            Range = unit.Range,
            CanAttack = unit.CanAttack,
            ProductionCost = unit.ProductionCost,
            PurchaseCost = unit.PurchaseCost,
            UpkeepCost = unit.UpkeepCost,
            Seed = unit.Seed,
        };
    }

    internal static UnitBase GetExampleUnit()
    {
        return CloneUnit(_exampleUnit);
    }
}
