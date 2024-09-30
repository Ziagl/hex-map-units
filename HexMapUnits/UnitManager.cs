using com.hexagonsimulations.Geometry.Hex;
using HexMapUnits.Enums;
using HexMapUnits.Models;

namespace HexMapUnits;

public class UnitManager
{
    private Dictionary<int, UnitBase> _unitStore = new();
    private int _lastUnitStoreId = 0;
    private MapData _map = new();
    private UnitFactory _factory;

    public UnitManager(List<List<int>> map, int rows, int columns, List<List<int>> notPassableTiles, List<UnitBase> unitDefinitions)
    {
        _map.Rows = rows;
        _map.Columns = columns;

        // create maps for each possible layer (sea, land, air)
        int layerIndex = 0;
        foreach (var layer in map)
        {
            List<int> layerMap = new();
            foreach(var tile in layer)
            {
                if(notPassableTiles.Count == map.Count && notPassableTiles[layerIndex].Contains(tile))
                {
                    layerMap.Add((int)TileType.UNPASSABLE);
                }
                else
                {
                    layerMap.Add((int)TileType.EMPTY);
                }
            }

            ++layerIndex;
            _map.Map.Add(layerMap);
        }

        // create factory
        _factory = new UnitFactory(unitDefinitions);
    }

    /// <summary>
    /// Creates a new unit at given layer, returns false if not possible
    /// </summary>
    /// <param name="unit">unit to create</param>
    /// <returns>true if unit was created, false if layer is invalid or position is already occupied</returns>
    public bool CreateUnit(UnitBase unit)
    {
        // early exit if layer is not valid
        if(unit.Layer < 0 || unit.Layer >= _map.Map.Count)
        {
            return false;
        }
        // early exit if layer position is already occupied
        var coordinates = unit.Position.ToOffset();
        var unitId = _map.Map[unit.Layer][coordinates.y * _map.Columns + coordinates.x];
        if (unitId != (int)TileType.EMPTY)
        {
            return false;
        }
        _factory.CreateUnit(unit);
        // add unit to store
        _lastUnitStoreId++;
        unit.Id = _lastUnitStoreId;
        _unitStore.Add(_lastUnitStoreId, unit);
        _map.Map[unit.Layer][coordinates.y * _map.Columns + coordinates.x] = unit.Id;
        return true;
    }

    /// <summary>
    /// Removes unit with given unit id from map
    /// </summary>
    /// <param name="unitId">id of unit</param>
    /// <returns>true if unit was removed, false if unit was not found</returns>
    public bool RemoveUnit(int unitId)
    {
        UnitBase? unit = null;
        if(!_unitStore.TryGetValue(unitId, out unit) || unit is null)
        {
            return false;
        }
        // remove from map
        var coordinates = unit.Position.ToOffset();
        _map.Map[unit.Layer][coordinates.y * _map.Columns + coordinates.x] = (int)TileType.EMPTY;
        // remove from store
        _unitStore.Remove(unitId);
        return true;
    }

    /// <summary>
    /// Moves a unit to destination (independent of movement costs)
    /// </summary>
    /// <param name="unitId">id of unit</param>
    /// <param name="destination">destination coordinates</param>
    /// <returns>true if unit was moved, false if unit was not found or destination is not passable</returns>
    public bool MoveUnit(int unitId, CubeCoordinates destination)
    {
        UnitBase? unit = null;
        if (!_unitStore.TryGetValue(unitId, out unit) || unit is null)
        {
            return false;
        }
        // early exit if destination is not passable
        var newCoordinates = destination.ToOffset();
        var oldCoordinates = unit.Position.ToOffset();
        if(_map.Map[unit.Layer][newCoordinates.y * _map.Columns + newCoordinates.x] != (int)TileType.EMPTY)
        {
            return false;
        }
        // remove unit from old position
        _map.Map[unit.Layer][oldCoordinates.y * _map.Columns + oldCoordinates.x] = (int)TileType.EMPTY;
        // set unit to new position
        _map.Map[unit.Layer][newCoordinates.y * _map.Columns + newCoordinates.x] = unit.Id;
        unit.Position = destination;
        return true;
    }

    /// <summary>
    /// Move unit by path and respects movement costs. Movement costs of starting
    /// position is not considered. If path is empty, unit is not moved.
    /// </summary>
    /// <param name="unitId">id of unit</param>
    /// <param name="path">path to destination</param>
    /// <returns>true if unit was moved, false if unit was not found, path is empty, path is too long or start is not given unit</returns>
    public bool MoveUnitByPath(int unitId, List<MovementTile> path)
    {
        UnitBase? unit = null;
        if(!_unitStore.TryGetValue(unitId, out unit))
        {
            return false;
        }
        // early exit if path is empty
        if(path.Count == 0) 
        {
            return false;
        }
        // early exit if start is not given unit
        if(unit.Position != path[0].Coordinates)
        {
            return false;
        }
        // early exit if destination is not passable
        var destinationCoords = path[^1].Coordinates.ToOffset();
        if (_map.Map[unit.Layer][destinationCoords.y * _map.Columns + destinationCoords.x] != (int)TileType.EMPTY)
        {
            return false;
        }
        // early exit if path costs too long for correct movement
        var movementCosts = 0;
        for(int i = 1; i < path.Count; i++)
        {
            movementCosts += path[i].Cost;
        }
        if(movementCosts > unit.Movement)
        {
            return false;
        }
        // remove unit from old position
        var oldCoordinates = unit.Position.ToOffset();
        _map.Map[unit.Layer][oldCoordinates.y * _map.Columns + oldCoordinates.x] = (int)TileType.EMPTY;
        // set unit to new position
        _map.Map[unit.Layer][destinationCoords.y * _map.Columns + destinationCoords.x] = unit.Id;
        unit.Position = path[^1].Coordinates;
        unit.Movement-= movementCosts;
        return true;
    }

    /// <summary>
    /// Get unit by id
    /// </summary>
    /// <param name="unitId">id of unit</param>
    /// <returns>unit if found, null if not found</returns>
    public UnitBase? GetUnitById(int unitId)
    {
        UnitBase? unit = null;
        if (_unitStore.TryGetValue(unitId, out unit))
        {
            return unit;
        }
        return null;
    }

    /// <summary>
    /// Get all units on this coordinates
    /// </summary>
    /// <param name="coordinates">coordinates to search for</param>
    /// <returns>array of units on this coordinates for given player, if no unit was found, empty array</returns>
    public List<UnitBase> GetUnitsByCoordinates(CubeCoordinates coordinates)
    {
        List<UnitBase> foundUnits = new();
        var coords = coordinates.ToOffset();
        foreach(var layer in _map.Map)
        {
            int tile = layer[coords.y * _map.Columns + coords.x];
            if (tile != (int)TileType.EMPTY && tile != (int)TileType.UNPASSABLE)
            {
                UnitBase? unit = null;
                if(_unitStore.TryGetValue(tile, out unit))
                {
                    foundUnits.Add(unit);
                }
            }
        }
        return foundUnits;
    }

    /// <summary>
    /// All units of given player number
    /// </summary>
    /// <param name="playerId">player id to search for</param>
    /// <returns>list of units for given player, if no unit was found, empty list</returns>
    public List<UnitBase> GetUnitsOfPlayer(int playerId)
    {
        List<UnitBase> foundUnits = new();
        foreach(var unit in _unitStore)
        {
            if(unit.Value.Player == playerId)
            {
                foundUnits.Add(unit.Value);
            }
        }
        return foundUnits;
    }
}
