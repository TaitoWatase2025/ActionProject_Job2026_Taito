using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [Header("ステータス")]
    public float maxHealth = 100;
    //public float maxCore = 50;  
    public float AttackPower = 10;
    public float health;
    //public float core;  
    public bool isDead = false;

    public event Action On10PercentHealthDown;
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;


    private HitStop hitstop;
    public float hitStopDuration = 0.1f;

    void Start()
    {
        health = maxHealth;
        hitstop = GetComponent<HitStop>();
    }
    public void TakeDamage(float amount, Vector3 attackOrigin)
    {
        if (health <= 0) return;

        health -= amount;
        health = Mathf.Max(health, 0);
        Debug.Log("ダメージを受けた: " + amount + " 残り体力: " + health);
        OnHealthChanged?.Invoke(health / maxHealth);
        if (health <= maxHealth * 0.1f)
        {
            On10PercentHealthDown?.Invoke();
        }
        if (health <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy Died");
        OnDeath?.Invoke();
    }
}
