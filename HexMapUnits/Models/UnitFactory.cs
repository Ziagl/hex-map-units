using com.hexagonsimulations.HexMapBase.Models;

namespace com.hexagonsimulations.HexMapUnits.Models;

public static class UnitFactory
{
    public static UnitBase CreateUnitBase(UnitType definition, int player, CubeCoordinates position, int layer)
        => new UnitBase
        {
            // ID is set by CreateUnit in UnitManager
            Player = player,
            Position = position,
            Layer = layer,
            Health = definition.MaxHealth,
            MaxHealth = definition.MaxHealth,
            Name = definition.Name,
            Images = new List<string>(definition.Images),
            Models = new List<string>(definition.Models),
            Description = definition.Description,
            Type = definition.Type,
            Era = definition.Era,
            MaxMovement = definition.MaxMovement,
            MovementType = definition.MovementType,
            Movement = definition.MaxMovement,
            WeaponType = definition.WeaponType,
            CombatStrength = definition.CombatStrength,
            Range = definition.Range,
            Fortification = definition.Fortification,
            Seed = new Random().Next(),
            Sight = definition.Sight,
            CanAttack = definition.CanAttack,
            CanBuildCity = definition.CanBuildCity,
            Goods = new Dictionary<int, int>(definition.Goods),
            ProductionCost = definition.ProductionCost,
            PurchaseCost = definition.PurchaseCost,
            UpkeepCost = definition.UpkeepCost,
        };
}
