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
        PlayArea.Instance.OnAttackFinished += PlayArea_OnAttackFinished;
        OnAttackStateStarted?.Invoke(this, EventArgs.Empty);
    }

    private void PlayArea_OnAttackFinished(object sender, EventArgs e)
    {
        PlayerStateMachine playerStateMachine = Player.Instance.gameObject.GetComponent<PlayerStateMachine>();

        playerStateMachine.SwitchState(new DecisionState(playerStateMachine, Player.Instance));
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        PlayArea.Instance.OnAttackFinished -= PlayArea_OnAttackFinished;
        //Debug.Log("Exit attack state");
    }

    
}
