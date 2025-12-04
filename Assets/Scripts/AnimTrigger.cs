using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    [SerializeField]
    AreaAttack areaAttack;
    [SerializeField]
    PushAttack pushAttack;
    public void OnAreaEfect()
    {
        areaAttack.OnAreaEfect();
    }
    public void OnPushEfect()
    {
        pushAttack.OnAreaEfect();
    }
}
