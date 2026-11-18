using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private GameOverUI gameOverUI;

    [SerializeField] private UpgradeConfig upgrades;

    private bool dead;

    public int Current { get; private set; }
    public int Max => maxHealth;

    public event Action<int, int> Changed;   // (현재, 최대)
    public event Action Died;

    private void Awake()
    {
        if (upgrades != null)
            maxHealth += Mathf.RoundToInt(upgrades.MaxHealthBonus);
        Current = maxHealth;
    }
    public void TakeDamage(int amount)
    {
        if (dead) return;
        Current = Mathf.Max(0, Current - amount);
        Changed?.Invoke(Current, maxHealth);
        if (Current <= 0)
        {
            dead = true;
            Died?.Invoke();
            if (gameOverUI != null) gameOverUI.Show();
        }
    }

    public void Heal(int amount)
    {
        Current = Mathf.Min(maxHealth, Current + amount);
        Changed?.Invoke(Current, maxHealth);
    }

    public void IncreaseMax(int amount)
    {
        maxHealth += amount;
        Heal(amount);
    }
}
