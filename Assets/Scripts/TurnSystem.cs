using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;
    public bool IsPlayersTurn { get; set; }
    public bool AttackedThisRound { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("You have another turn system in your scene");
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        AttackState.OnAttackStateStarted += AttackState_OnAttackStateStarted;
        IsPlayersTurn = true;
    }

    private void AttackState_OnAttackStateStarted(object sender, EventArgs e)
    {
        AttackedThisRound = true;
    }
}
