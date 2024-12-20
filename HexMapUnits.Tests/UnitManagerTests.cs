using com.hexagonsimulations.HexMapBase.Geometry.Hex;
using com.hexagonsimulations.HexMapBase.Geometry.Hex.Models;
using com.hexagonsimulations.HexMapUnits.Models;

namespace com.hexagonsimulations.HexMapUnits.Tests;

[TestClass]
public sealed class UnitManagerTests
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
        CanBuildCity = false,
        Goods = new() { { 1, 2 } },
        ProductionCost = 0,
        PurchaseCost = 0,
    };

    public UnitManagerTests()
    {
        _unitDefinitions.Add(new UnitBase() { });
    }

    [TestMethod]
    public void TestCreateUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        Assert.AreNotEqual(0, unit.Id);
    }

    [TestMethod]
    public void TestCreateUnitInvalidLayer()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        unit.Layer = 1;
        bool success = unitManager.CreateUnit(unit);
        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestCreateUnitPositionOccupied()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        exampleMap[0] = 1;
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>() { new List<int>() { 1 } }, _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestCreateUnitAddTwoUnitsOnSameCoordinate()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var unit2 = CloneUnit(_exampleUnit);
        success = unitManager.CreateUnit(unit2);
        Assert.IsFalse(success);
    }

    [TestMethod]
    public void TestGetUnitsOfPlayer()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        unit.Player = 1;
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var unit2 = CloneUnit(_exampleUnit);
        unit2.Player = 2;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.IsTrue(success);
        var unit3 = CloneUnit(_exampleUnit);
        unit3.Player = 1;
        unit3.Position = new CubeCoordinates(2, 0, -2);
        success = unitManager.CreateUnit(unit3);
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsOfPlayer(2);
        Assert.AreEqual(1, units.Count);
        Assert.AreEqual(2, units.First().Id);
        units = unitManager.GetUnitsOfPlayer(1);
        Assert.AreEqual(2, units.Count);
    }

    [TestMethod]
    public void TestRemoveUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        success = unitManager.RemoveUnit(unit.Id);
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsOfPlayer(1);
        Assert.AreEqual(0, units.Count);
    }

    [TestMethod]
    public void TestGetUnitById()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var unit2 = unitManager.GetUnitById(unit.Id);
        Assert.IsNotNull(unit2);
        Assert.AreEqual(unit.Id, unit2.Id);
    }

    [TestMethod]
    public void TestGetUnitsByCoordinates()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.AreEqual(1, units.Count);
        Assert.AreEqual(1, units.First().Id);
    }

    [TestMethod]
    public void TestMoveUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        success = unitManager.MoveUnit(unit.Id, new CubeCoordinates(1, 0, -1));
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(1, 0, -1));
        Assert.AreEqual(1, units.Count);
        Assert.AreEqual(1, units.First().Id);
        units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.AreEqual(0, units.Count);
    }

    [TestMethod]
    public void TestMoveUnitByPath()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = CloneUnit(_exampleUnit);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        success = unitManager.MoveUnitByPath(unit.Id, new List<WeightedCubeCoordinates>() {
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(0, 0, 0),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(1, 0, -1),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(2, 0, -2),
                                           Cost = 5 },
        });
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(2, 0, -2));
        Assert.AreEqual(1, units.Count);
        Assert.AreEqual(1, units.First().Id);
        units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.AreEqual(0, units.Count);
    }

    [TestMethod]
    public void TestIsTileOccupied()
    {
        var map = new List<List<int>>() {
            Enumerable.Repeat(0, 16).ToList(),  // layer 0
            Enumerable.Repeat(0, 16).ToList()   // layer 1
        };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit1 = CloneUnit(_exampleUnit);
        unit1.Player = 1;
        unit1.Layer = 0;
        unit1.Position = new CubeCoordinates(0, 0, 0);
        bool success = unitManager.CreateUnit(unit1);
        Assert.IsTrue(success);
        var unit2 = CloneUnit(_exampleUnit);
        unit2.Player = 2;
        unit2.Layer = 0;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.IsTrue(success);
        var unit3 = CloneUnit(_exampleUnit);
        unit3.Player = 1;
        unit3.Layer = 1;
        unit3.Position = new CubeCoordinates(2, 0, -2);
        success = unitManager.CreateUnit(unit3);
        Assert.IsTrue(success);
        var selectedUnit = CloneUnit(_exampleUnit);
        selectedUnit.Player = 1;
        selectedUnit.Layer = 0;
        selectedUnit.CanAttack = false;
        selectedUnit.Position = new CubeCoordinates(1, 1, -2);
        success = unitManager.CreateUnit(selectedUnit);
        Assert.IsTrue(success);
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(0, 0, 0), selectedUnit));   // occupied by unit1, can not attack
        selectedUnit.CanAttack = true;
        Assert.IsTrue(unitManager.IsTileOccupied(new CubeCoordinates(0, 0, 0), selectedUnit));    // occupied by unit1, can attack
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(1, 0, -1), selectedUnit));  // unit2 with different player
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(2, 0, -2), selectedUnit));  // unit3 with different layer
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(0, 1, -1), selectedUnit));  // empty tile
    }

    [TestMethod]
    public void TestCanAttack()
    {
        var map = new List<List<int>>() {
            Enumerable.Repeat(0, 16).ToList(),  // layer 0
            Enumerable.Repeat(0, 16).ToList()   // layer 1
        };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit1 = CloneUnit(_exampleUnit);
        unit1.Player = 1;
        unit1.Layer = 0;
        unit1.Position = new CubeCoordinates(0, 0, 0);
        bool success = unitManager.CreateUnit(unit1);
        Assert.IsTrue(success);
        var unit2 = CloneUnit(_exampleUnit);
        unit2.Player = 2;
        unit2.Layer = 0;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.IsTrue(success);
        var unit3 = CloneUnit(_exampleUnit);
        unit3.Player = 2;
        unit3.Layer = 1;
        unit3.Position = new CubeCoordinates(2, 0, -2);
        success = unitManager.CreateUnit(unit3);
        Assert.IsTrue(success);
        var selectedUnit = CloneUnit(_exampleUnit);
        selectedUnit.Player = 1;
        selectedUnit.Layer = 0;
        selectedUnit.CanAttack = false;
        selectedUnit.Position = new CubeCoordinates(1, 1, -2);
        success = unitManager.CreateUnit(selectedUnit);
        Assert.IsTrue(success);
        Assert.IsFalse(unitManager.CanAttack(new CubeCoordinates(1, 0, -1), selectedUnit));        // enemy unit, but canAttack is false
        selectedUnit.CanAttack = true;
        Assert.IsTrue(unitManager.CanAttack(new CubeCoordinates(1, 0, -1), selectedUnit));        // enemy unit, but canAttack is true
        Assert.IsFalse(unitManager.CanAttack(new CubeCoordinates(0, 0, 0), selectedUnit));        // unit of same player
        Assert.IsFalse(unitManager.CanAttack(new CubeCoordinates(2, 0, -2), selectedUnit));       // enemy unit with different layer
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
