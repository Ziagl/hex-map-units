using com.hexagonsimulations.Geometry.Hex;
using HexMapUnits.Enums;

namespace HexMapUnits.Models;

public record UnitBase
{
    // base
    public int Id; // id = index in unit store
    public int Player; // id of player this unit belongs to
    public UnitType Type; // the type of this unit (value of an enum?)
    public int Health; // current health points
    public int MaxHealth; // maximum health points
    public int Movement; // current movement points
    public int MaxMovement; // maximum movement points (affects movement range)
    // position
    public CubeCoordinates Position; // its position on the map
    public int Layer; // index of layer this unit is on
    // stats
    public int Attack; // attack points (damage in fight)
    public int Defense; // defence points (how much damage is reduced)
    public int Range; // attack range (how far can this unit attack)
    // flags
    public bool CanAttack; // can this unit attack?
    // economy
    public int ProductionCost; // amount of production needed to build this unit
    public int PurchaseCost; // amount of gold needed to purchase this unit
}
