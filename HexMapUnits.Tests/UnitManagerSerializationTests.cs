using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapUnits.Models;
using HexMapUnits.Tests;
using System.Reflection;

namespace com.hexagonsimulations.HexMapUnits.Tests;

[TestClass]
public sealed class UnitManagerSerializationTests
{
    private readonly string TempDir = @"C:\Temp\";
    private readonly bool DumpToDisk = false; // set to true to dump serialized data to disk for inspection

    private readonly List<UnitBase> _unitDefinitions = new();

    public UnitManagerSerializationTests()
    {
        _unitDefinitions.Add(new UnitBase() { });
    }

    [TestMethod]
    public void UnitManager_Json()
    {
        var map = new List<List<int>>() { Enumerable.Repeat(0, 16).ToList() };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>() { });
        var unit1 = TestUtils.GetExampleUnit();
        var unit2 = TestUtils.GetExampleUnit();
        unit2.Player = 2;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        unitManager.CreateUnit(unit1);
        unitManager.CreateUnit(unit2);

        var json = unitManager.ToJson();
        Assert.IsFalse(string.IsNullOrWhiteSpace(json), "JSON should not be empty.");

        if (DumpToDisk)
        {
            File.WriteAllText($"{TempDir}UnitManager.json", json);
        }

        var roundTripped = UnitManager.FromJson(json);
        Assert.IsNotNull(roundTripped, "Deserialized UnitManager should not be null.");

        AssertUnitManagerEqual(unitManager, roundTripped);
    }

    [TestMethod]
    public void UnitManager_Binary()
    {
        var map = new List<List<int>>() { Enumerable.Repeat(0, 16).ToList() };
        var unitManager = new UnitManager(map, 4, 4, new List<List<int>>() { });
        var unit1 = TestUtils.GetExampleUnit();
        var unit2 = TestUtils.GetExampleUnit();
        unit2.Player = 2;
        unit2.Position = new CubeCoordinates(1, 0, -1);
        unitManager.CreateUnit(unit1);
        unitManager.CreateUnit(unit2);

        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            unitManager.Write(writer);
        }

        if (DumpToDisk)
        {
            File.WriteAllBytes($"{TempDir}UnitManager.bin", ms.ToArray());
        }

        ms.Position = 0;
        UnitManager roundTripped;
        using (var reader = new BinaryReader(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            roundTripped = UnitManager.Read(reader);
        }

        Assert.IsNotNull(roundTripped, "Binary deserialized UnitManager should not be null.");
        AssertUnitManagerEqual(unitManager, roundTripped);
    }

    private static void AssertUnitManagerEqual(UnitManager expected, UnitManager actual)
    {
        Assert.IsNotNull(expected, "Expected manager is null.");
        Assert.IsNotNull(actual, "Actual manager is null.");

        // Compare JSON canonical form as a fast full-object check
        var expectedJson = expected.ToJson();
        var actualJson = actual.ToJson();
        Assert.AreEqual(expectedJson, actualJson, "Serialized JSON of UnitManagers differ.");

        // Compare the private _unitStore dictionaries
        var expectedStore = GetUnitStore(expected);
        var actualStore = GetUnitStore(actual);

        Assert.HasCount(expectedStore.Count, actualStore, "Unit store counts differ.");

        foreach (var kvp in expectedStore.OrderBy(k => k.Key))
        {
            Assert.IsTrue(actualStore.ContainsKey(kvp.Key), $"Actual unit store missing key {kvp.Key}.");
            var expectedUnit = kvp.Value;
            var actualUnit = actualStore[kvp.Key];
            AssertUnitEqual(expectedUnit, actualUnit, $"Unit with Id {kvp.Key} mismatch.");
        }
    }

    private static Dictionary<int, UnitBase> GetUnitStore(UnitManager manager)
    {
        var field = typeof(UnitManager).GetField("_unitStore", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, "_unitStore field not found on UnitManager.");
        var value = field!.GetValue(manager) as Dictionary<int, UnitBase>;
        Assert.IsNotNull(value, "_unitStore field value is null.");
        return value!;
    }

    private static void AssertUnitEqual(UnitBase expected, UnitBase actual, string context)
    {
        Assert.IsNotNull(expected, $"Expected unit null: {context}");
        Assert.IsNotNull(actual, $"Actual unit null: {context}");
        Assert.AreEqual(expected.Id, actual.Id, $"{context} Id mismatch.");
        Assert.AreEqual(expected.Player, actual.Player, $"{context} Player mismatch.");
        Assert.AreEqual(expected.Health, actual.Health, $"{context} Health mismatch.");
        Assert.AreEqual(expected.MaxHealth, actual.MaxHealth, $"{context} MaxHealth mismatch.");
        Assert.AreEqual(expected.Name, actual.Name, $"{context} Name mismatch.");
        SequenceEqual(expected.Images, actual.Images, $"{context} Images mismatch.");
        Assert.AreEqual(expected.Description, actual.Description, $"{context} Description mismatch.");
        Assert.AreEqual(expected.Type, actual.Type, $"{context} Type mismatch.");
        Assert.AreEqual(expected.Era, actual.Era, $"{context} Era mismatch.");
        Assert.AreEqual(expected.MaxMovement, actual.MaxMovement, $"{context} MaxMovement mismatch.");
        Assert.AreEqual(expected.MovementType, actual.MovementType, $"{context} MovementType mismatch.");
        Assert.AreEqual(expected.Movement, actual.Movement, $"{context} Movement mismatch.");
        Assert.AreEqual(expected.WeaponType, actual.WeaponType, $"{context} WeaponType mismatch.");
        Assert.AreEqual(expected.CombatStrength, actual.CombatStrength, $"{context} CombatStrength mismatch.");
        Assert.AreEqual(expected.RangedAttack, actual.RangedAttack, $"{context} RangedAttack mismatch.");
        Assert.AreEqual(expected.Range, actual.Range, $"{context} Range mismatch.");
        Assert.AreEqual(expected.Fortification, actual.Fortification, $"{context} Fortification mismatch.");
        Assert.AreEqual(expected.Seed, actual.Seed, $"{context} Seed mismatch.");
        Assert.AreEqual(expected.Sight, actual.Sight, $"{context} Sight mismatch.");
        Assert.AreEqual(expected.CanAttack, actual.CanAttack, $"{context} CanAttack mismatch.");
        Assert.AreEqual(expected.CanBuildCity, actual.CanBuildCity, $"{context} CanBuildCity mismatch.");
        DictionaryEqual(expected.Goods, actual.Goods, $"{context} Goods mismatch.");
        Assert.AreEqual(expected.ProductionCost, actual.ProductionCost, $"{context} ProductionCost mismatch.");
        Assert.AreEqual(expected.PurchaseCost, actual.PurchaseCost, $"{context} PurchaseCost mismatch.");
        Assert.AreEqual(expected.UpkeepCost, actual.UpkeepCost, $"{context} UpkeepCost mismatch.");
        AssertCubeCoordinatesEqual(expected.Position, actual.Position, $"{context} Position mismatch.");
        Assert.AreEqual(expected.Layer, actual.Layer, $"{context} Layer mismatch.");
    }

    private static void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string context)
    {
        if (expected == null && actual == null) return;
        Assert.IsNotNull(expected, $"{context} expected sequence null.");
        Assert.IsNotNull(actual, $"{context} actual sequence null.");
        var eList = expected.ToList();
        var aList = actual.ToList();
        Assert.HasCount(eList.Count, aList, $"{context} count mismatch.");
        for (int i = 0; i < eList.Count; i++)
            Assert.AreEqual(eList[i], aList[i], $"{context} element {i} mismatch.");
    }

    private static void DictionaryEqual<TKey, TValue>(IDictionary<TKey, TValue> expected, IDictionary<TKey, TValue> actual, string context)
    {
        if (expected == null && actual == null) return;
        Assert.IsNotNull(expected, $"{context} expected dictionary null.");
        Assert.IsNotNull(actual, $"{context} actual dictionary null.");
        Assert.HasCount(expected.Count, actual, $"{context} count mismatch.");
        foreach (var kvp in expected)
        {
            Assert.IsTrue(actual.ContainsKey(kvp.Key), $"{context} missing key {kvp.Key}.");
            Assert.AreEqual(kvp.Value, actual[kvp.Key], $"{context} value mismatch for key {kvp.Key}.");
        }
    }

    private static void AssertCubeCoordinatesEqual(CubeCoordinates expected, CubeCoordinates actual, string context)
    {
        Assert.IsNotNull(expected, $"{context} expected null.");
        Assert.IsNotNull(actual, $"{context} actual null.");
        if (Equals(expected, actual)) return;

        var eType = expected.GetType();
        var props = eType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var p in props.Where(p => p.CanRead))
        {
            var ev = p.GetValue(expected);
            var av = p.GetValue(actual);
            Assert.AreEqual(ev, av, $"{context} coordinate component {p.Name} mismatch.");
        }

        var fields = eType.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var f in fields)
        {
            var ev = f.GetValue(expected);
            var av = f.GetValue(actual);
            Assert.AreEqual(ev, av, $"{context} coordinate field {f.Name} mismatch.");
        }
    }
}
