namespace HexMapUnits.Models;

public class CombatModificators
{
    public int TerrainBonus = 0;       // tile bonus positive or negative (for example hills)
    public int FortificationBonus = 0; // fortification bonus only positive (for example not moving units)
    public int WeaponBonus = 0;        // weapon bonus positive or negative (depending on weapon types of both units, for example cavalry vs artillery)
    public bool RangedAttack = false;  // if this is an ranged attack, attacker will not receive damage
}
