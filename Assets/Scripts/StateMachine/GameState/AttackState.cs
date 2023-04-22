using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackState : BaseGameState
{
    public static event EventHandler OnAttackStateStarted;

    public AttackState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        PlayArea.Instance.AllCardsAttack(TurnSystem.Instance.IsPlayersTurn);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerStateMachine playerStateMachine = playerObject.GetComponent<PlayerStateMachine>();
        Player player = playerObject.GetComponent<Player>();

        TurnSystem.Instance.AttackedThisRound = true;

        OnAttackStateStarted?.Invoke(this, EventArgs.Empty);

        playerStateMachine.SwitchState(new DecisionState(playerStateMachine, player));

    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Debug.Log("Exit attack state");
    }

    
}
