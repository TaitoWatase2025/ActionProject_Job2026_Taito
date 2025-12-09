using UnityEngine;

public class ClearAreaTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        Debug.Log("[HiddenClear] プレイヤーがエリアに侵入");

        FadeController.Instance.StartHiddenClearFade();
    }
}

