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
            unit.Attack = definition.Attack;
            unit.Defense = definition.Defense;
            unit.Range = definition.Range;
            unit.CanAttack = definition.CanAttack;
            unit.ProductionCost = definition.ProductionCost;
            unit.PurchaseCost = definition.PurchaseCost;
        }
    }
}
