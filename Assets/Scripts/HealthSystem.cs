using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    [SerializeField] private int currentHealth = 100;

    public void Damage(int damageAmt)
    {
        currentHealth -= damageAmt;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }
}
