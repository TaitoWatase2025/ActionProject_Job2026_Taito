using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class HitStop : MonoBehaviour
{
    private bool isHitStopped = false;

    public IEnumerator DoHitStop(float duration)
    {
        if (isHitStopped) yield break;
        isHitStopped = true;

        // Animatorí‚é~
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var a in animators) a.speed = 0f;

        // NavMeshAgentí‚é~
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float agentSpeed = 0f;
        if (agent != null)
        {
            agentSpeed = agent.speed;
            agent.speed = 0f;
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(duration);

        // çƒäJ
        foreach (var a in animators) a.speed = 1f;
        if (agent != null)
        {
            agent.speed = agentSpeed;
            agent.isStopped = false;
        }

        isHitStopped = false;
    }
}

