# AGENTS.md - HexMapUnits Library Context

## Project Overview

**HexMapUnits** is a .NET library for managing units in tile-based (hexagon map) games. It provides data structures and logic for unit management, movement, combat, and serialization.

- **Namespace**: `com.hexagonsimulations.HexMapUnits`
- **Current Version**: 0.5.0
- **Target Framework**: .NET 10.0
- **Package**: Published as NuGet package
- **Repository**: https://github.com/Ziagl/hex-map-units
- **Dependencies**: HexMapBase v0.5.0 (provides coordinate systems and interfaces)
- **Author**: Werner Ziegelwanger
- **Company**: Hexagon Simulations

## Architecture

### Core Components

1. **UnitManager** (`UnitManager.cs`)
   - Central manager for all unit operations
   - Manages unit lifecycle: creation, movement, removal
   - Handles map layers (sea, land, air)
   - Combat calculation system
   - Serialization support (JSON and binary)
   - Key methods:
     - `CreateUnit()` - Creates a new unit at given layer, validates position
     - `RemoveUnit()` - Removes unit by ID from map and store
     - `MoveUnit()` - Moves unit to destination (ignores movement costs)
     - `MoveUnitByPath()` - Moves unit following path, respects movement costs
     - `GetUnitById()` - Retrieves unit by ID
     - `GetUnitsByCoordinates()` - Gets all units at specific coordinates
     - `GetUnitsOfPlayer()` - Gets all units belonging to a player
     - `IsTileOccupied()` - Checks if tile is occupied by same player or enemy
     - `CanAttack()` - Validates if unit can attack given coordinates
     - `IsTilePassable()` - Checks if tile is passable for unit
     - `GetTileStatus()` - Returns tile state (empty, unpassable, or unit ID)
     - `ComputeCombatOutcome()` - Calculates combat damage for attacker and defender
     - `ToJson()` / `FromJson()` - JSON serialization
     - `Write()` / `Read()` - Binary serialization

2. **Models** (`Models/`)
   - **UnitBase**: Core unit entity with properties:
     - **Identity**: Id, Player, Health, MaxHealth
     - **Description**: Name, Images (List<string>), Description, Type, Era
     - **Movement**: MaxMovement, MovementType, Movement (current points)
     - **Combat**: WeaponType, CombatStrength, RangedAttack, Range, Fortification
     - **Economy**: Goods (Dictionary<int,int>), ProductionCost, PurchaseCost, UpkeepCost
     - **Position**: Position (CubeCoordinates), Layer (int - map layer index)
     - **Scouting**: Sight (vision range)
     - **Flags**: CanAttack, CanBuildCity
     - **Random**: Seed (for combat RNG determinism)
     - Implements: `ICombatEntity` from HexMapBase
   
   - **UnitFactory**: Internal factory for creating units from definitions
     - Stores unit definitions/templates
     - Applies template properties to new units
     - Initializes random seed on creation
   
   - **MapData**: Internal map representation
     - Properties: Rows, Columns, Map (List<List<int>>)
     - Each layer is a flat list representing 2D grid
     - Provides `Clone()` for deep copying
   
   - **CombatModificators**: Combat calculation modifiers
     - AttackerTerrainBonus / DefenderTerrainBonus
     - AttackerFortificationBonus / DefenderFortificationBonus
     - AttackerWeaponBonus / DefenderWeaponBonus
     - RangedAttack (bool - no counter-attack if true)
     - NoCounterAttack (bool - for cities/civilian units)

3. **Enums** (`Enums/`)
   - **TileType**: Internal enum
     - `UNPASSABLE = -1` - Blocked tile
     - `EMPTY = 0` - Vacant tile

### External Dependencies

- **HexMapBase** library provides:
  - `CubeCoordinates`: Hexagonal coordinate system (x, y, z where x+y+z=0)
  - `OffsetCoordinates`: Alternative coordinate representation (x, y)
  - `WeightedCubeCoordinates`: Pathfinding support (Coordinates + Cost)
  - `ICombatEntity`: Combat interface

## Design Patterns & Conventions

### Coordinate System
- Uses **cube coordinates** from HexMapBase for hexagonal tiles
- Convertible to offset coordinates via `ToOffset()`
- 3D coordinate system: x + y + z = 0
- Offset coordinates used for map array indexing: `[y * columns + x]`

### Map Layers
- Multi-layer system (typically: sea layer, land layer, air layer)
- Each layer is a flat list representing a 2D grid
- Tile access: `map[layer][y * columns + x]`
- Tile values:
  - `-1` (UNPASSABLE): Blocked tile
  - `0` (EMPTY): Vacant tile
  - `> 0`: Unit ID occupying tile

### Unit Management
- Units stored in dictionary by ID (`_unitStore`)
- Unit IDs are auto-incremented integers starting from 1
- Unit position tracked both in UnitBase and map grid
- Factory pattern for unit creation from definitions
- Constructor takes unit definitions for factory initialization

### Movement System
- Two movement modes:
  1. **Direct movement** (`MoveUnit`): Teleport to destination, ignores costs
  2. **Path-based movement** (`MoveUnitByPath`): Respects movement costs and remaining points
- Movement points decrease with each move
- Special rule: Allow last tile if movement points > 0 but < tile cost (partial movement)
- Exception: Tiles with `int.MaxValue` cost (rivers) don't get partial movement allowance
- Path validation: Must start at unit's current position

### Combat System
- Combat formula: `damage = 30.0 * exp(0.04 * strengthDiff) * randomFactor`
- Random factor: 0.8 to 1.2 (±20% variance)
- Strength calculation: CombatStrength + terrain + fortification + weapon bonuses
- Ranged attacks add RangedAttack to attacker strength
- Seeded RNG for deterministic combat (critical for multiplayer sync)
- Seeds updated after combat for next calculation
- Supports:
  - Ranged attacks (no counter-attack)
  - Counter-attacks (default melee behavior)
  - No counter-attack mode (for city/civilian targets)

### Serialization
- **JSON** (System.Text.Json):
  - Human-readable format
  - Uses intermediate `UnitManagerState` class
  - Auto-repairs map/unit position inconsistencies on load
  
- **Binary** (BinaryReader/BinaryWriter):
  - Compact format with version control (current: version 1)
  - Explicit field serialization
  - Position serialized as JSON string for safety
  - Auto-repairs map/unit position inconsistencies on load

## Testing

### Test Project
- **HexMapUnits.Tests** using MSTest
- Target framework: .NET 10.0
- Test files:
  - `UnitManagerTests.cs`: Core functionality tests (35+ test cases)
  - `UnitManagerSerializationTests.cs`: JSON/binary serialization tests
  - `TestUtils.cs`: Helper methods for test data generation
  - `MSTestSettings.cs`: Test configuration

### Test Coverage Areas
- Unit creation (valid, invalid layer, occupied positions)
- Unit removal
- Movement (direct and path-based with various scenarios)
- Unit queries (by ID, coordinates, player)
- Tile status checks (occupied, passable, attackable)
- Multi-layer interactions
- Combat calculations (damage ranges, ranged attacks, no counter-attack)
- Serialization round-trips (JSON and binary)

### Test Utilities
- `TestUtils.GetExampleUnit()`: Creates sample unit with default values
- Example unit defaults: Player=1, Health=100, Movement=10, Layer=0, Position=(0,0,0)

## Common Tasks

### Adding New Unit Properties
1. Add property to `UnitBase` class in `Models/UnitBase.cs`
2. Update `UnitFactory.CreateUnit()` in `Models/UnitFactory.cs` if definition-based
3. Update serialization methods in `UnitManager.cs`:
   - `WriteUnit()` - Add binary write call
   - `ReadUnit()` - Add binary read call
   - JSON usually works automatically due to System.Text.Json
4. Add tests for new property in `UnitManagerTests.cs`
5. Consider if property needs cloning in copy scenarios

### Modifying Combat Logic
1. Edit `ComputeCombatOutcome()` in `UnitManager.cs`
2. Update `CombatModificators` in `Models/CombatModificators.cs` if new modifiers needed
3. Test with various scenarios in `UnitManagerTests.ComputeCombatOutcome()`
4. Verify random seed handling for multiplayer consistency
5. Document combat formula changes in comments

### Adding New Movement Rules
1. Modify `MoveUnit()` or `MoveUnitByPath()` in `UnitManager.cs`
2. Update `IsTilePassable()` or `IsTileOccupied()` if needed
3. Add corresponding test cases in `UnitManagerTests.cs`
4. Consider edge cases: layer boundaries, occupied tiles, movement points

### Extending Map Layer Logic
1. Adjust layer initialization in `UnitManager` constructor
2. Update layer-specific logic in movement/combat methods
3. Consider cross-layer interactions (air vs. ground combat) - currently TODO in `CanAttack()`
4. Update validation checks for layer bounds
5. Test multi-layer scenarios

### Versioning & Releases
1. Update `<Version>` in `HexMapUnits.csproj`
2. Update binary serialization version if format changes
3. Maintain backward compatibility or provide migration path
4. Update README.md with release notes
5. Tag release in git repository

## Code Style & Patterns

- **Visibility**: Internal types exposed to tests via `InternalsVisibleTo` in `Properties/AssemblyInfo.cs`
- **Null Safety**: Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- **Early Exits**: Pattern of early returns for validation (improves readability)
- **Immutable Defaults**: Use `new()` for collection initialization
- **Cloning**: Deep clones for MapData to prevent mutation
- **Dictionary Usage**: TryGetValue pattern for safe lookups
- **LINQ**: Used for collection operations (Select, ToList, etc.)
- **Error Handling**: Return bool success/failure rather than exceptions for game logic

## File Organization

```
HexMapUnits/
├── HexMapUnits.csproj      # Project configuration, NuGet metadata
├── UnitManager.cs          # Main manager class (600+ lines)
├── Models/
│   ├── UnitBase.cs         # Unit entity (public)
│   ├── UnitFactory.cs      # Unit creation (internal)
│   ├── MapData.cs          # Map state (internal)
│   └── CombatModificators.cs  # Combat modifiers (public)
├── Enums/
│   └── TileType.cs         # Tile state enum (internal)
└── Properties/
    └── AssemblyInfo.cs     # Test visibility configuration

HexMapUnits.Tests/
├── HexMapUnits.Tests.csproj
├── UnitManagerTests.cs             # Core functionality tests
├── UnitManagerSerializationTests.cs # Serialization tests
├── TestUtils.cs                     # Test helpers
└── MSTestSettings.cs                # Test configuration
```

## Known Patterns & Gotchas

1. **Movement Points**: Units start with `MaxMovement` points; must be reset manually per turn (no auto-reset)
2. **Seed Management**: Combat updates unit seeds for next RNG; critical for multiplayer determinism
3. **Layer Validation**: Always validate layer bounds before map access (0 <= layer < map.Count)
4. **Occupancy Logic**: `IsTileOccupied()` has `considerCanAttack` parameter:
   - `true` (default): Enemy tiles moveable if unit can attack
   - `false`: Ignores attack capability, used for placement
5. **Path Start Validation**: `MoveUnitByPath()` requires path[0] == unit.Position
6. **Tile Cost Exception**: `int.MaxValue` cost (rivers) has special handling - no partial movement
7. **Map Repair**: Deserialization auto-repairs map/unit position mismatches for data integrity
8. **Coordinate Conversion**: Always convert CubeCoordinates to Offset before indexing map array
9. **Multi-layer TODO**: Cross-layer combat (air vs. ground) not fully implemented - see comment in `CanAttack()`
10. **Empty Path**: `MoveUnitByPath()` with empty path returns false without moving unit

## Build & Deployment

- **Solution**: HexMapUnits.sln
- **Build Configurations**: Debug, Release
- **Target Framework**: net10.0
- **Output**: `bin/{Debug|Release}/net10.0/`
- **NuGet Packaging**: Automated via .csproj properties
- **Version Bumps**: Update `<Version>` in HexMapUnits.csproj
- **Package Metadata**: Title, Description, Tags, Icon, License all in .csproj
- **Dependencies**: HexMapBase v0.5.0 via PackageReference

## API Usage Examples

### Initializing UnitManager
```csharp
// Create 4x4 map with one layer
List<int> tiles = Enumerable.Repeat(0, 16).ToList();
var map = new List<List<int>>() { tiles };
var notPassable = new List<List<int>>() { new List<int> { 5 } }; // tile type 5 is impassable
var unitDefinitions = new List<UnitBase>(); // unit templates

var unitManager = new UnitManager(map, rows: 4, columns: 4, notPassable, unitDefinitions);
```

### Creating and Moving Units
```csharp
var unit = new UnitBase 
{ 
    Player = 1, 
    Position = new CubeCoordinates(0, 0, 0),
    Layer = 0,
    Movement = 10 
};

unitManager.CreateUnit(unit); // Returns true if successful
unitManager.MoveUnit(unit.Id, new CubeCoordinates(1, 0, -1)); // Direct move
```

### Combat
```csharp
var attacker = unitManager.GetUnitById(1);
var defender = unitManager.GetUnitById(2);
var mods = new CombatModificators 
{ 
    DefenderTerrainBonus = 2, // defender on hills
    DefenderFortificationBonus = 5 // defender fortified
};

var (attackerDamage, defenderDamage) = unitManager.ComputeCombatOutcome(attacker, defender, mods);
attacker.Health -= attackerDamage;
defender.Health -= defenderDamage;
```

### Serialization
```csharp
// Save state
string json = unitManager.ToJson();
File.WriteAllText("save.json", json);

// Load state
string json = File.ReadAllText("save.json");
var unitManager = UnitManager.FromJson(json);

// Binary (more compact)
using var writer = new BinaryWriter(File.OpenWrite("save.bin"));
unitManager.Write(writer);

using var reader = new BinaryReader(File.OpenRead("save.bin"));
var unitManager = UnitManager.Read(reader);
```

## Future Considerations

- **Cross-layer Combat**: TODO comment in `CanAttack()` indicates need for air-to-ground combat
- **Turn Management**: Movement point reset not implemented (game-specific logic)
- **Unit Abilities**: No special abilities system yet (workers, scouts, etc.)
- **Zone of Control**: No implementation of ZOC movement rules
- **Stacking**: Currently one unit per tile per layer
- **Experience/Promotion**: No unit leveling system
- **Supply Lines**: No supply/logistics system
- **AI Pathfinding**: Library provides data, but AI logic external
