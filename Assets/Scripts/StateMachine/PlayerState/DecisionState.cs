using UnityEngine;
using System;

public class DecisionState : BasePlayerState
{
    public static event EventHandler OnEnterDecisionState;
    public static event EventHandler OnExitDecisionState;

    public DecisionState(PlayerStateMachine stateMachine, Player player) : base(stateMachine, player) { }

    Hand playerHand;

    public override void Enter()
    {
        //Debug.Log($"BEGIN DecisionState for '{player}'");
        OnEnterDecisionState?.Invoke(this, EventArgs.Empty);
        AssignPlayerHand();
    }


    public override void Tick()
    {
        playerHand.ClickToUnselectCard();
        playerHand.SelectCard();
        playerHand.PlaceCard();

        if (Bell.Instance.CheckIfClickBell())
        {
            if (TurnSystem.Instance.AttackedThisRound)
            {
                GameStateMachine gameState = GameObject.FindObjectOfType<GameStateMachine>();
                
                gameState.SwitchState(new StartRoundState(gameState));
            }

            stateMachine.SwitchState(new WaitingState(stateMachine, player));
        }
    }

    public override void Exit()
    {
        OnExitDecisionState?.Invoke(this, EventArgs.Empty);
        //Debug.Log($"BEGIN DecisionState for '{player}'");
    }

    private void AssignPlayerHand()
    {
        foreach (Hand hand in Utils.GetHands())
        {
            if (hand.BelongsToPlayer)
            {
                playerHand = hand;
            }
        }
    }
}

