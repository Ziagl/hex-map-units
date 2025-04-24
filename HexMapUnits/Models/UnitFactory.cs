namespace com.hexagonsimulations.HexMapUnits.Models;

internal class UnitFactory
{
    private List<UnitBase> _unitDefinitions = new();

    internal UnitFactory(List<UnitBase> unitDefinitions)
    {
        _unitDefinitions = unitDefinitions;
    }

    public void CreateUnit(UnitBase unit)
    {
        var definition = _unitDefinitions.Find((definition) => definition.Type == unit.Type);
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
