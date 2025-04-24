namespace HexMapUnits.Models;

public class CombatModificators
{
    public int AttackerTerrainBonus = 0; // tile bonus positive or negative (for example hills)
    public int DefenderTerrainBonus = 0;
    public int AttackerFortificationBonus = 0; // fortification bonus only positive (for example not moving units)
    public int DefenderFortificationBonus = 0;
    public int AttackerWeaponBonus = 0; // weapon bonus positive or negative (depending on weapon types of both units, for example cavalry vs artillery)
    public int DefenderWeaponBonus = 0;
    public bool RangedAttack = false;  // if this is an ranged attack, attacker will not receive damage
}
