using com.hexagonsimulations.Geometry.Hex;
using com.hexagonsimulations.Geometry.Hex.Models;
using HexMapUnits.Models;

namespace HexMapUnits.Tests;

public class UnitManagerTests
{
    private readonly List<UnitBase> _unitDefinitions = new();
    private readonly UnitBase _exampleUnit = new()
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
        Attack = 10,
        RangedAttack = 5,
        Defense = 10,
        Range = 1,
        CanAttack = false,
        Goods = new() { { 1, 2 } },
        ProductionCost = 0,
        PurchaseCost = 0,
    };

    public UnitManagerTests()
    {
        _unitDefinitions.Add(new UnitBase() { });
    }

    [Fact]
    public void TestCreateUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        Assert.NotEqual(0, unit.Id);
    }

    [Fact]
    public void TestCreateUnitInvalidLayer()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        unit.Layer = 1;
        bool success = unitManager.CreateUnit(unit);
        Assert.False(success);
    }

    [Fact]
    public void TestCreateUnitPositionOccupied()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        exampleMap[0] = 1;
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>() { new List<int>() { 1 } }, _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.False(success);
    }

    [Fact]
    public void TestCreateUnitAddTwoUnitsOnSameCoordinate()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        var unit2 = CloneUnit(_exampleUnit);
        success = unitManager.CreateUnit(unit2);
        Assert.False(success);
    }

    [Fact]
    public void TestGetUnitsOfPlayer()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        unit.Player = 1;
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        var unit2 = CloneUnit(_exampleUnit);
        unit2.Player = 2;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.True(success);
        var unit3 = CloneUnit(_exampleUnit);
        unit3.Player = 1;
        unit3.Position = new CubeCoordinates(2, 0, -2);
        success = unitManager.CreateUnit(unit3);
        Assert.True(success);
        var units = unitManager.GetUnitsOfPlayer(2);
        Assert.Single(units);
        Assert.Equal(2, units.First().Id);
        units = unitManager.GetUnitsOfPlayer(1);
        Assert.Equal(2, units.Count);
    }

    [Fact]
    public void TestRemoveUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        success = unitManager.RemoveUnit(unit.Id);
        Assert.True(success);
        var units = unitManager.GetUnitsOfPlayer(1);
        Assert.Empty(units);
    }

    [Fact]
    public void TestGetUnitById()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        var unit2 = unitManager.GetUnitById(unit.Id);
        Assert.NotNull(unit2);
        Assert.Equal(unit.Id, unit2.Id);
    }

    [Fact]
    public void TestGetUnitsByCoordinates()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.Single(units);
        Assert.Equal(1, units.First().Id);
    }

    [Fact]
    public void TestMoveUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        success = unitManager.MoveUnit(unit.Id, new CubeCoordinates(1, 0, -1));
        Assert.True(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(1, 0, -1));
        Assert.Single(units);
        Assert.Equal(1, units.First().Id);
        units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.Empty(units);
    }

    [Fact]
    public void TestMoveUnitByPath()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.True(success);
        success = unitManager.MoveUnitByPath(unit.Id, new List<WeightedCubeCoordinates>() {
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(0, 0, 0),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(1, 0, -1),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(2, 0, -2),
                                           Cost = 5 },
        });
        Assert.True(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(2, 0, -2));
        Assert.Single(units);
        Assert.Equal(1, units.First().Id);
        units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.Empty(units);
    }

    private UnitBase CloneUnit(UnitBase unit)
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
            Attack = unit.Attack,
            Defense = unit.Defense,
            Range = unit.Range,
            CanAttack = unit.CanAttack,
            ProductionCost = unit.ProductionCost,
            PurchaseCost = unit.PurchaseCost
        };
    }
}
