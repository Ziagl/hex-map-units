using System.Text.Json.Serialization;

namespace com.hexagonsimulations.HexMapUnits.Models;

internal class UnitFactory
{
    private List<UnitBase> _unitDefinitions = new();

    // Needed for System.Text.Json
    [JsonConstructor]
    public UnitFactory() { }

    internal UnitFactory(List<UnitBase> unitDefinitions)
    {
        _unitDefinitions = unitDefinitions ?? new();
    }

    // Exposed so serializer can populate
    public List<UnitBase> UnitDefinitions
    {
        get => _unitDefinitions;
        set => _unitDefinitions = value ?? new();
    }

    public void CreateUnit(UnitBase unit)
    {
        var definition = _unitDefinitions.Find(d => d.Type == unit.Type);
        if (definition is not null)
        {
            unit.MaxMovement = definition.MaxMovement;
            unit.WeaponType = definition.WeaponType;
            unit.CombatStrength = definition.CombatStrength;
            unit.Range = definition.Range;
            unit.Sight = definition.Sight;
            unit.CanAttack = definition.CanAttack;
            unit.ProductionCost = definition.ProductionCost;
            unit.PurchaseCost = definition.PurchaseCost;
            unit.UpkeepCost = definition.UpkeepCost;
            unit.Seed = new Random().Next();
        }
    }
}
