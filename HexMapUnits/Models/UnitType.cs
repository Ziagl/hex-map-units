using System.Text.Json.Serialization;

namespace com.hexagonsimulations.HexMapUnits.Models;

public record UnitType
{
    // base entity
    [JsonPropertyName("health")]
    public int Health { get; set; }
    [JsonPropertyName("maxHealth")]
    public int MaxHealth { get; set; }
    // base unit
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty; // the name of this unit type
    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new(); //representation of this unit in UI and/or map
    [JsonPropertyName("models")]
    public List<string> Models { get; set; } = new(); // 3D models for this unit (if used in 3D)
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty; // short description for what this unit is used
    [JsonPropertyName("type")]
    public int Type { get; set; } // the type of this unit (value of an enum?)
    [JsonPropertyName("era")]
    public int Era { get; set; } // min era for this unit
    [JsonPropertyName("maxMovement")]
    public int MaxMovement { get; set; } // maximum movement points (affects movement range)
    [JsonPropertyName("movementType")]
    public int MovementType { get; set; } // type of unit (f.e. move, wheel, shallow water, deep water, air, ...)
    [JsonPropertyName("movement")]
    public int Movement { get; set; } // current movement points
    // combat
    [JsonPropertyName("weaponType")]
    public int WeaponType { get; set; } // type of weapon/combat of this unit (infantry, cavalry, ...)
    [JsonPropertyName("combatStrength")]
    public int CombatStrength { get; set; } // attack and defense points (damage in fight)
    [JsonPropertyName("rangedAttack")]
    public int RangedAttack { get; set; } // ranged attack points (damage of airstrike)
    [JsonPropertyName("range")]
    public int Range { get; set; } // attack range (how far can this unit attack)
    [JsonPropertyName("fortification")]
    public int Fortification { get; set; } // fortification level (if entity does not move +1 level)
    // random number
    [JsonPropertyName("seed")]
    public int Seed { get; set; } // random number seed for this unit for this turn
    // scouting
    [JsonPropertyName("sight")]
    public int Sight { get; set; } // how far can this unit see (in hexes)
    // flags
    [JsonPropertyName("canAttack")]
    public bool CanAttack { get; set; } // can this unit attack?
    [JsonPropertyName("canBuildCity")]
    public bool CanBuildCity { get; set; } // can this unit build a new city?
    // economy
    [JsonPropertyName("goods")]
    public Dictionary<int, int> Goods { get; set; } = new(); // goods needed to produce this unit
    [JsonPropertyName("productionCost")]
    public int ProductionCost { get; set; } // amount of production needed to build this unit
    [JsonPropertyName("purchaseCost")]
    public int PurchaseCost { get; set; } // amount of gold needed to purchase this unit
    [JsonPropertyName("upkeepCost")]
    public int UpkeepCost { get; set; } // amount of gold needed to upkeep this unit
}
