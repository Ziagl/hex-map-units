using com.hexagonsimulations.Geometry.Hex.Interfaces;
using com.hexagonsimulations.HexMapBase.Models;

namespace com.hexagonsimulations.HexMapUnits.Models;

public class UnitBase : GameEntity
{
    // base
    public string Name = string.Empty; // the name of this unit type
    public List<string> Images = new(); //representation of this unit in UI and/or map
    public string Description = string.Empty; // short description for what this unit is used
    public int Type; // the type of this unit (value of an enum?)
    public int Era; // min era for this unit
    public int MaxMovement; // maximum movement points (affects movement range)
    public int MovementType; // type of unit (f.e. move, wheel, shallow water, deep water, air, ...)
    public int Movement; // current movement points
    // stats
    public int WeaponType; // type of weapon/combat of this unit (infantry, cavalry, ...)
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
    // position
    public CubeCoordinates Position; // its position on the map
    public int Layer; // index of layer this unit is on
}
