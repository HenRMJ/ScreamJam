using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseGameState
{
    public AttackState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        PlayArea.Instance.AllCardsAttack(TurnSystem.Instance.IsPlayersTurn);
        if (TurnSystem.Instance.IsPlayersTurn)
        {

        }
    }

    public override void Tick()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    
}
