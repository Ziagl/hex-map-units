using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapUnits.Models;
using HexMapUnits.Models;
using HexMapUnits.Tests;

namespace com.hexagonsimulations.HexMapUnits.Tests;

[TestClass]
public sealed class UnitManagerTests
{
    private readonly List<UnitBase> _unitDefinitions = new();

    public UnitManagerTests()
    {
        _unitDefinitions.Add(new UnitBase() { });
    }

    [TestMethod]
    public void CreateUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        Assert.AreNotEqual(0, unit.Id);
    }

    [TestMethod]
    public void CreateUnitInvalidLayer()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        unit.Layer = 1;
        bool success = unitManager.CreateUnit(unit);
        Assert.IsFalse(success);
    }

    [TestMethod]
    public void CreateUnitPositionOccupied()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        exampleMap[0] = 1;
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>() { new List<int>() { 1 } }, _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        bool success = unitManager.CreateUnit(unit);
        Assert.IsFalse(success);
    }

    [TestMethod]
    public void CreateUnitAddTwoUnitsOnSameCoordinate()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var unit2 = TestUtils.GetExampleUnit();
        success = unitManager.CreateUnit(unit2);
        Assert.IsFalse(success);
    }

    [TestMethod]
    public void GetUnitsOfPlayer()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        unit.Player = 1;
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var unit2 = TestUtils.GetExampleUnit();
        unit2.Player = 2;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.IsTrue(success);
        var unit3 = TestUtils.GetExampleUnit();
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
    public void RemoveUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        success = unitManager.RemoveUnit(unit.Id);
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsOfPlayer(1);
        Assert.AreEqual(0, units.Count);
    }

    [TestMethod]
    public void GetUnitById()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var unit2 = unitManager.GetUnitById(unit.Id);
        Assert.IsNotNull(unit2);
        Assert.AreEqual(unit.Id, unit2.Id);
    }

    [TestMethod]
    public void GetUnitsByCoordinates()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.AreEqual(1, units.Count);
        Assert.AreEqual(1, units.First().Id);
    }

    [TestMethod]
    public void MoveUnit()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
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
    public void MoveUnitByPath()
    {
        List<int> exampleMap = Enumerable.Repeat(0, 16).ToList();
        var unitManager = new UnitManager(new List<List<int>>() { exampleMap }, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        success = unitManager.MoveUnitByPath(unit.Id, new List<WeightedCubeCoordinates>() {
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(0, 0, 0),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(1, 0, -1),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(2, 0, -2),
                                           Cost = 5 },
        }, new CubeCoordinates(2, 0, -2));
        Assert.IsTrue(success);
        var units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(2, 0, -2));
        Assert.AreEqual(1, units.Count);
        Assert.AreEqual(1, units.First().Id);
        units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.AreEqual(0, units.Count);
        Assert.AreEqual(0, unit.Movement);
        // move back
        unit.Movement += 5;
        success = unitManager.MoveUnitByPath(unit.Id, new List<WeightedCubeCoordinates>() {
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(2, 0, -2),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(1, 0, -1),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(0, 0, 0),
                                           Cost = 5 },
        }, new CubeCoordinates(2, 0, -2));
        Assert.IsFalse(success);
        success = unitManager.MoveUnitByPath(unit.Id, new List<WeightedCubeCoordinates>() {
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(2, 0, -2),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(1, 0, -1),
                                           Cost = 5 },
            new WeightedCubeCoordinates(){ Coordinates = new CubeCoordinates(0, 0, 0),
                                           Cost = 5 },
        }, new CubeCoordinates(1, 0, -1));
        Assert.IsTrue(success);
        units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(1, 0, -1));
        Assert.AreEqual(1, units.Count);
        Assert.AreEqual(1, units.First().Id);
        units = unitManager.GetUnitsByCoordinates(new CubeCoordinates(0, 0, 0));
        Assert.AreEqual(0, units.Count);
        Assert.AreEqual(0, unit.Movement);
    }

    [TestMethod]
    public void IsTileOccupied()
    {
        var map = new List<List<int>>() {
            Enumerable.Repeat(0, 16).ToList(),  // layer 0
            Enumerable.Repeat(0, 16).ToList()   // layer 1
        };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit1 = TestUtils.GetExampleUnit();
        unit1.Player = 1;
        unit1.Layer = 0;
        unit1.Position = new CubeCoordinates(0, 0, 0);
        bool success = unitManager.CreateUnit(unit1);
        Assert.IsTrue(success);
        var unit2 = TestUtils.GetExampleUnit();
        unit2.Player = 2;
        unit2.Layer = 0;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.IsTrue(success);
        var unit3 = TestUtils.GetExampleUnit();
        unit3.Player = 1;
        unit3.Layer = 1;
        unit3.Position = new CubeCoordinates(2, 0, -2);
        success = unitManager.CreateUnit(unit3);
        Assert.IsTrue(success);
        var selectedUnit = TestUtils.GetExampleUnit();
        selectedUnit.Player = 1;
        selectedUnit.Layer = 0;
        selectedUnit.CanAttack = false;
        selectedUnit.Position = new CubeCoordinates(1, 1, -2);
        success = unitManager.CreateUnit(selectedUnit);
        Assert.IsTrue(success);
        Assert.IsTrue(unitManager.IsTileOccupied(new CubeCoordinates(1, 0, -1), selectedUnit));         // occupied by enemy unit, can not attack
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(1, 0, -1), selectedUnit, false));  // occupied by enemy unit, can attack, do not consider can attack flag
        selectedUnit.CanAttack = true;
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(1, 0, -1), selectedUnit));        // occupied by enemy unit, can attack
        Assert.IsTrue(unitManager.IsTileOccupied(new CubeCoordinates(0, 0, 0), selectedUnit));          // unit1 with same player
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(2, 0, -2), selectedUnit));        // unit3 with different layer
        Assert.IsFalse(unitManager.IsTileOccupied(new CubeCoordinates(0, 1, -1), selectedUnit));        // empty tile
    }

    [TestMethod]
    public void CanAttack()
    {
        var map = new List<List<int>>() {
            Enumerable.Repeat(0, 16).ToList(),  // layer 0
            Enumerable.Repeat(0, 16).ToList()   // layer 1
        };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>(), _unitDefinitions);
        var unit1 = TestUtils.GetExampleUnit();
        unit1.Player = 1;
        unit1.Layer = 0;
        unit1.Position = new CubeCoordinates(0, 0, 0);
        bool success = unitManager.CreateUnit(unit1);
        Assert.IsTrue(success);
        var unit2 = TestUtils.GetExampleUnit();
        unit2.Player = 2;
        unit2.Layer = 0;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.IsTrue(success);
        var unit3 = TestUtils.GetExampleUnit();
        unit3.Player = 2;
        unit3.Layer = 1;
        unit3.Position = new CubeCoordinates(2, 0, -2);
        success = unitManager.CreateUnit(unit3);
        Assert.IsTrue(success);
        var selectedUnit = TestUtils.GetExampleUnit();
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

    [TestMethod]
    public void IsTilePassable()
    {
        var map = new List<List<int>>() {
            Enumerable.Repeat(0, 16).ToList(),  // layer 0
            Enumerable.Repeat(5, 16).ToList()   // layer 1, impassable
        };
        var tile = new CubeCoordinates(1, 0, -1);
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>() { new List<int>() { 5 }, new List<int>() { 5 } } , _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        unit.Player = 1;
        unit.Layer = 0;
        unit.Position = new CubeCoordinates(0, 0, 0);
        bool success = unitManager.CreateUnit(unit);
        Assert.IsTrue(success);
        // Test passable tile
        bool isPassable = unitManager.IsTilePassable(tile, unit);
        Assert.IsTrue(isPassable);
        // Test impassable tile
        unit.Layer = 1;
        isPassable = unitManager.IsTilePassable(tile, unit);
        Assert.IsFalse(isPassable);
        unit.Layer = 0;
        // Test tile occupied by another unit
        var unit2 = TestUtils.GetExampleUnit();
        unit2.Player = 2;
        unit2.Layer = 0;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        success = unitManager.CreateUnit(unit2);
        Assert.IsTrue(success);
        isPassable = unitManager.IsTilePassable(tile, unit);
        Assert.IsFalse(isPassable);
    }

    [TestMethod]
    public void GetTileStatus()
    {
        var map = new List<List<int>>() { Enumerable.Repeat(0, 16).ToList() };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>() { }, _unitDefinitions);
        var unit = TestUtils.GetExampleUnit();
        unit.Id = 1;
        unit.Position = new CubeCoordinates(0, 0, 0);
        unitManager.CreateUnit(unit);
        unit = TestUtils.GetExampleUnit();
        unit.Id = 2;
        unit.Position = new CubeCoordinates(1, 0, -1);
        unitManager.CreateUnit(unit);
        int status = unitManager.GetTileStatus(new CubeCoordinates(-1, 0, 1), 0);
        Assert.AreEqual(-2, status, $"Coordinates should be wrong, got {status}");
        status = unitManager.GetTileStatus(new CubeCoordinates(0, 0, 0), -1);
        Assert.AreEqual(-3, status, $"Layer should be wrong, got {status}");
        status = unitManager.GetTileStatus(new CubeCoordinates(2, 0, -2), 0);
        Assert.AreEqual(0, status, $"Tile should be empty, got {status}");
        status = unitManager.GetTileStatus(new CubeCoordinates(0, 0, 0), 0);
        Assert.AreEqual(1, status, $"Tile should be id of first unit, got {status}");
        status = unitManager.GetTileStatus(new CubeCoordinates(1, 0, -1), 0);
        Assert.AreEqual(2, status, $"Tile should be id of second unit, got {status}");
    }

    [TestMethod]
    public void ComputeCombatOutcome()
    {
        var random = new Random();
        var map = new List<List<int>>() { Enumerable.Repeat(0, 16).ToList() };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>() { }, _unitDefinitions);
        var attacker = TestUtils.GetExampleUnit();
        var defender = TestUtils.GetExampleUnit();
        attacker.Seed = random.Next();
        defender.Seed = random.Next();
        var mods = new CombatModificators();
        // combat between units that are equally strong results in values
        // between 24 and 36 (depending on random value)
        for (int i = 0; i < 10; ++i)
        {
            var output = unitManager.ComputeCombatOutcome(attacker, defender, mods);
            Assert.IsTrue(output.damageAttacker >= 24 && output.damageAttacker <= 36);
            Assert.IsTrue(output.damageDefender >= 24 && output.damageDefender <= 36);
        }
        // ranged attack and no counter attack leads to 0 damage for attacking units
        mods.RangedAttack = true;
        var result = unitManager.ComputeCombatOutcome(attacker, defender, mods);
        Assert.AreEqual(0, result.damageAttacker, "Attacker received damage, but it was a ranged attack.");
        mods.RangedAttack = false;
        mods.NoCounterAttack = true;
        result = unitManager.ComputeCombatOutcome(attacker, defender, mods);
        Assert.AreEqual(0, result.damageAttacker, "attacker received damage, but NoCounterAttack was true.");
    }
}
