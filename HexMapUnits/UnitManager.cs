using com.hexagonsimulations.HexMapBase.Interfaces;
using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapUnits.Enums;
using com.hexagonsimulations.HexMapUnits.Models;
using HexMapUnits.Models;

namespace com.hexagonsimulations.HexMapUnits;

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
    /// <param name="target">position the unit should move to</param>
    /// <returns>true if unit was moved, false if unit was not found, path is empty, path is too long or start is not given unit</returns>
    public bool MoveUnitByPath(int unitId, List<WeightedCubeCoordinates> path, CubeCoordinates target)
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
        var destinationCoords = target.ToOffset();
        if (_map.Map[unit.Layer][destinationCoords.y * _map.Columns + destinationCoords.x] != (int)TileType.EMPTY)
        {
            return false;
        }
        // early exit if target is not in weighted Path
        bool found = false;
        foreach(var tile in path)
        {
            if(tile.Coordinates == target)
            {
                found = true;
                break;
            }
        }
        if(!found)
        {
            return false;
        }
        // early exit if path costs too long for correct movement
        var movementCosts = 0;
        var index = 0;
        for(int i = 1; i < path.Count; i++)
        {
            index = i;
            movementCosts += path[i].Cost;
            if(target == path[i].Coordinates)
            {
                break;
            }
        }
        // special case to allow last item if movement points are left, but not enough
        // to fully fit last tile or if last tile was a river tile
        if(movementCosts > unit.Movement + 1 && path[index].Cost != int.MaxValue)
        {
            return false;
        }
        // remove unit from old position
        var oldCoordinates = unit.Position.ToOffset();
        _map.Map[unit.Layer][oldCoordinates.y * _map.Columns + oldCoordinates.x] = (int)TileType.EMPTY;
        // set unit to new position
        _map.Map[unit.Layer][destinationCoords.y * _map.Columns + destinationCoords.x] = unit.Id;
        unit.Position = target;
        unit.Movement-= movementCosts;
        if(unit.Movement < 0)
        {
            unit.Movement = 0;
        }
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

    /// <summary>
    /// Checks if given coordinates is occupied by unit on same layer and of same player
    /// </summary>
    /// <param name="coordinates">tile coordinates to check</param>
    /// <param name="unit">unit with additional data</param>
    /// <param name="considerCanAttack">by default canAttack is considered, but it can be turned of (for example to place units)</param>
    /// <returns>true if a unit o given player is on this tile otherwiese false</returns>
    public bool IsTileOccupied(CubeCoordinates coordinates, UnitBase unit, bool considerCanAttack = true)
    {
        var offsetCoordinates = coordinates.ToOffset();
        int unitId = _map.Map[unit.Layer][offsetCoordinates.y * _map.Columns + offsetCoordinates.x];
        // early exit if map position is empty
        if(unitId == (int)TileType.EMPTY)
        {
            return false;
        }
        // get unit and check for detail (same player)
        var otherUnit = GetUnitById(unitId);
        if (otherUnit != null &&
           otherUnit.Layer == unit.Layer &&
           otherUnit.Player == unit.Player)
        {
            
            return true;
        }
        // get unit and check for detail (for other player)
        if (considerCanAttack)
        {
            if (otherUnit != null &&
               otherUnit.Layer == unit.Layer &&
               otherUnit.Player != unit.Player)
            {
                // if unit can attack, enemy unit tile is moveable
                // if unit can not attack, it is not possible to move to occupied fields
                return !unit.CanAttack;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if given unit can attack given coordinates.
    /// </summary>
    /// <param name="coordinates">Tile coordinates that should checked for enemy unit</param>
    /// <param name="unit">Unit who wants to attack</param>
    /// <returns>true if coordinates contains attackable units otherwise false</returns>
    public bool CanAttack(CubeCoordinates coordinates, UnitBase unit)
    {
        // early exit if unit is not able to attack
        if(unit.CanAttack == false)
        {
            return false;
        }
        var offsetCoordinates = coordinates.ToOffset();
        int unitId = _map.Map[unit.Layer][offsetCoordinates.y * _map.Columns + offsetCoordinates.x];
        // early exit if map position is empty
        if (unitId == (int)TileType.EMPTY)
        {
            return false;
        }
        // get unit and check for detail
        var otherUnit = GetUnitById(unitId);
        if (otherUnit != null &&
           otherUnit.Layer == unit.Layer &&         // Todo: this is only for combat on same layer -> consider also trans layer combat!
           otherUnit.Player != unit.Player)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if given tile position is passable for this unit.
    /// </summary>
    /// <param name="unit">unit that should be placed on given coordinates</param>
    /// <param name="coordinates">tile coordinates that should be checked</param>
    /// <returns></returns>
    public bool IsTilePassable(CubeCoordinates coordinates, UnitBase unit)
    {
        var offSetcoordinates = coordinates.ToOffset();
        var tileInfo = _map.Map[unit.Layer][offSetcoordinates.y * _map.Columns + offSetcoordinates.x];
        return tileInfo == (int)TileType.EMPTY;
    }

    /// <summary>
    /// Get tile status of given coordinates.
    /// </summary>
    /// <param name="coordinates">Map coordinates</param>
    /// <param name="layer">Map layer</param>
    /// <returns>Returns ether TileType, the unitId this tile belongs, 
    /// -2 if given coordinates are not on map or -3 if layer is not known.</returns>
    public int GetTileStatus(CubeCoordinates coordinates, int layer)
    {
        var offsetCoordinates = coordinates.ToOffset();
        // early exit if given coordinates are not on map
        if (offsetCoordinates.x < 0 || offsetCoordinates.x >= _map.Columns ||
            offsetCoordinates.y < 0 || offsetCoordinates.y >= _map.Rows)
        {
            return -2;
        }
        // early exit if layer is not valid
        if (layer < 0 || layer >= _map.Map.Count)
        {
            return -3;
        }
        return _map.Map[layer][offsetCoordinates.y * _map.Columns + offsetCoordinates.x];
    }

    /// <summary>
    /// Computes the combat outcome of two units.
    /// </summary>
    /// <param name="attacker">Attacker unit</param>
    /// <param name="defender">Defender unit</param>
    /// <param name="mods">Modification for this fight (environment, technology, ...)</param>
    /// <returns>Computed damage values for given combat inputs.</returns>
    public (int damageAttacker, int damageDefender) ComputeCombatOutcome(ICombatEntity attacker, ICombatEntity defender, CombatModificators mods)
    {
        // basic combat algorithm
        int damageDefender = (attacker.Attack + mods.AttackerBaseStrength + mods.AttackerSurfaceBonus) - defender.Defense - mods.DefenderSurfaceBonus;
        if (damageDefender < 0)
        {
            damageDefender = 0;
        }
        int damageAttacker = (defender.Attack + mods.DefenderBaseStrength) - attacker.Defense;
        if (damageAttacker < 0)
        {
            damageAttacker = 0;
        }
        // TODO other fancy stuff
        return (damageAttacker, damageDefender);
    }
}
