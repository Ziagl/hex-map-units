using com.hexagonsimulations.Geometry.Hex;

namespace HexMapUnits.Models;

public record MovementTile
{
    public CubeCoordinates Coordinates;
    public int Cost;
}
