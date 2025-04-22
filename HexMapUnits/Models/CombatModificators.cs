namespace HexMapUnits.Models;

public class CombatModificators
{
    public int AttackerBaseStrength = 0;    // base values for computation algorithm that can be set from outside (f.e. 10 attack ...
    public int DefenderBaseStrength = 0;    // ... and 5 defense), so an attacker always makes damage if other mods are 0)
    public int AttackerSurfaceBonus = 0;    // tile bonus that makes the attack stronger (for example hills)
    public int DefenderSurfaceBonus = 0;    // tile bonus that makes the attack weaker (for example forest)
    public int AttackerTechnologyBonus = 0; // technology bonus that makes the attack stronger (sharper blades)
    public int DefenderTechnologyBonus = 0; // technology bonus that makes the attack weaker (thicker armor)
}
