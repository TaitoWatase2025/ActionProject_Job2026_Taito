using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    [SerializeField]
    AreaAttack areaAttack;
    public void OnAreaEfect()
    {
        areaAttack.OnAreaEfect();
    }
}
