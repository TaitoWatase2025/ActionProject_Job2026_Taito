using UnityEngine;

public class HitboxController : MonoBehaviour
{
    public SwordAttack swordAttack; // Inspector Ç≈ê›íË
    public AreaAttack areaAttack; // Inspector Ç≈ê›íË

    public void EnableSwordCollider()
    {
        if (swordAttack != null)
            swordAttack.EnableCollider();
    }
    public void DisableSwordCollider()
    {
        if (swordAttack != null)
            swordAttack.DisableCollider();
    }
    public void EnableAreaCollider()
    {
        if (areaAttack != null)
            areaAttack.EnableCollider();
    }
    public void DisableAreaCollider()
    {
        if (areaAttack != null)
            areaAttack.DisableCollider();
    }
}
