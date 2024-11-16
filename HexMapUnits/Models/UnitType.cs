namespace HexMapUnits.Models;

public record UnitType
{
    // base
    public string Name = string.Empty; // the name of this unit type
    public string Description = string.Empty; // short description for what this unit is used
    public int Type; // the type of this unit (value of an enum?)
    public int Era; // min era for this unit
    public int MaxHealth; // maximum health points
    public int MaxMovement; // maximum movement points (affects movement range)
    // stats
    public int Attack; // attack points (damage in fight)
    public int RangedAttack; // ranged attack points (damage of airstrike)
    public int Defense; // defence points (how much damage is reduced)
    public int Range; // attack range (how far can this unit attack)
    // flags
    public bool CanAttack; // can this unit attack?
    public bool CanBuildCity; // can this unit build a new city?
    // economy
    public Dictionary<int, int> Goods = new(); // goods needed to produce this unit
    public int ProductionCost; // amount of production needed to build this unit
    public int PurchaseCost; // amount of gold needed to purchase this unit
}
