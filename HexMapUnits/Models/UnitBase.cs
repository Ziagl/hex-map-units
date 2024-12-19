using com.hexagonsimulations.Geometry.Hex;

namespace com.hexagonsimulations.HexMapUnits.Models;

public record UnitBase : UnitType
{
    // base
    public int Id; // id = index in unit store
    public int Player; // id of player this unit belongs to
    public int Health; // current health points
    public int Movement; // current movement points
    // position
    public CubeCoordinates Position; // its position on the map
    public int Layer; // index of layer this unit is on
}
