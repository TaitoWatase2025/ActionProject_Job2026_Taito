using UnityEngine;

public class HitboxController : MonoBehaviour
{
    public SwordAttack swordAttack; // Inspector Ç≈ê›íË

    // Animation Event Ç≈åƒÇ‘
    public void EnableSwordCollider()
    {
        if (swordAttack != null)
            swordAttack.EnableCollider();
    }

    // Animation Event Ç≈åƒÇ‘
    public void DisableSwordCollider()
    {
        if (swordAttack != null)
            swordAttack.DisableCollider();
    }
}
