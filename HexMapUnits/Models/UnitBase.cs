using com.hexagonsimulations.HexMapBase.Interfaces;
using com.hexagonsimulations.HexMapBase.Models;

namespace com.hexagonsimulations.HexMapUnits.Models;

public class UnitBase : ICombatEntity
{
    // base entity
    public int Id { get; set; }
    public int Player { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    // base unit
    public string Name { get; set; } = string.Empty; // the name of this unit type
    public List<string> Images { get; set; } = new(); //representation of this unit in UI and/or map
    public string Description { get; set; } = string.Empty; // short description for what this unit is used
    public int Type { get; set; } // the type of this unit (value of an enum?)
    public int Era { get; set; } // min era for this unit
    public int MaxMovement { get; set; } // maximum movement points (affects movement range)
    public int MovementType { get; set; } // type of unit (f.e. move, wheel, shallow water, deep water, air, ...)
    public int Movement { get; set; } // current movement points
    // combat
    public int WeaponType { get; set; } // type of weapon/combat of this unit (infantry, cavalry, ...)
    public int Attack { get; set; } // attack points (damage in fight)
    public int RangedAttack { get; set; } // ranged attack points (damage of airstrike)
    public int Defense { get; set; } // defence points (how much damage is reduced)
    public int Range { get; set; } // attack range (how far can this unit attack)
    // flags
    public bool CanAttack { get; set; } // can this unit attack?
    public bool CanBuildCity { get; set; } // can this unit build a new city?
    // economy
    public Dictionary<int, int> Goods { get; set; } = new(); // goods needed to produce this unit
    public int ProductionCost { get; set; } // amount of production needed to build this unit
    public int PurchaseCost { get; set; } // amount of gold needed to purchase this unit
    // position
    public CubeCoordinates Position { get; set; } // its position on the map
    public int Layer { get; set; } // index of layer this unit is on
}
