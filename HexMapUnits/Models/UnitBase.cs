using com.hexagonsimulations.HexMapBase.Interfaces;
using com.hexagonsimulations.HexMapBase.Models;
using System.Text.Json.Serialization;

namespace com.hexagonsimulations.HexMapUnits.Models;

public record UnitBase : UnitType, ICombatEntity
{
    // base entity
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("player")]
    public int Player { get; set; }
    // map information
    [JsonPropertyName("position")]
    public CubeCoordinates Position { get; set; } // its position on the map
    [JsonPropertyName("layer")]
    public int Layer { get; set; } // index of layer this unit is on

    // Internal constructor - only accessible within the same assembly
    // This allows UnitFactory to create instances while preventing external code from doing so
    // JSON deserializer can still access it since it's in the same assembly
    [JsonConstructor]
    internal UnitBase()
    {
    }
}
