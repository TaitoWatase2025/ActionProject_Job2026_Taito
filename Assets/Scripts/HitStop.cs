using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HitStop : MonoBehaviour
{
    public IEnumerator Stop(float duration)
    {
        float timer = 0f;

        // Animator/移動を停止
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.speed = 0;

        CharacterController cc = GetComponent<CharacterController>();
        NavMeshAgent nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (nav != null) nav.velocity = Vector3.zero;

        // ★ 実時間で停止（Time.timeScale 使わない）
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // 再開
        if (anim != null) anim.speed = 1;
    }
}


