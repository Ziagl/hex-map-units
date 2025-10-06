namespace com.hexagonsimulations.HexMapBase.Models;

// Made serializable by keeping it as a simple POCO with settable properties.
internal class MapData
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public List<List<int>> Map { get; set; } = new();

    public MapData() { }

    public MapData Clone()
    {
        return new MapData
        {
            Rows = Rows,
            Columns = Columns,
            Map = Map.Select(layer => new List<int>(layer)).ToList()
        };
    }
}
