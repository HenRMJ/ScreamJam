using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseGameState
{
    public AttackState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        PlayArea.Instance.AllCardsAttack(TurnSystem.Instance.IsPlayersTurn);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerStateMachine playerStateMachine = playerObject.GetComponent<PlayerStateMachine>();
        Player player = playerObject.GetComponent<Player>();

        TurnSystem.Instance.AttackedThisRound = true;
        
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
