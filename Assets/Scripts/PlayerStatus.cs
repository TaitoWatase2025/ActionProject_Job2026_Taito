using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("ステータス")]
    public float maxHealth = 100;
    //public float maxCore = 50;
    public float attackPower = 50;
    public float health;
    //public float core;
    public float StunTime = 0f;

    public event Action<float> OnHealthChanged;
    public event Action Ondeath;

    void Start()
    {
        health = maxHealth;
    }
    void Update()
    {
        if (StunTime > 0)
        {
            StunTime -= Time.deltaTime;
        }
    }
    public bool IsStunned()
    {
        return StunTime > 0;
    }
    public void TakeDamage(float amount)
    {
        if (health <= 0) return;
        health -= amount;
        health = Mathf.Max(health, 0);
        OnHealthChanged?.Invoke(health / maxHealth);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        Ondeath?.Invoke();
    }
}
