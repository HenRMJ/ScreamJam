using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public static event EventHandler OnEnemyDied;

    [SerializeField] private int blood;
    [SerializeField] private Hand hand;

    public int Blood { get; set; }

    private void Awake()
    {
        Blood = blood;
    }

    private void Start()
    {
        AttackState.OnAttackStateStarted += AttackState_OnAttackStateStarted;
    }

    private void AttackState_OnAttackStateStarted(object sender, EventArgs e)
    {
        if (blood <= 0)
        {
            OnEnemyDied?.Invoke(this, EventArgs.Empty);
        }
    }
}
