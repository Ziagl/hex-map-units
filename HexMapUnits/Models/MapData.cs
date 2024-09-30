internal record MapData
{
    public List<List<int>> Map { get; set; } = new();
    public int Rows { get; set; }
    public int Columns { get; set; }
}
