using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPhase : PhaseBase
{
    public static event EventHandler OnEnterDecisionState;
    public static event EventHandler OnExitDecisionState;

    private Hand playerHand;

    public override void EnterState(PhaseStateManager boss)
    {
        OnEnterDecisionState?.Invoke(this, EventArgs.Empty);
        AssignPlayerHand();
    }

    public override void Tick(PhaseStateManager boss)
    {
        playerHand.ClickToUnselectCard();
        playerHand.SelectCard();
        playerHand.PlaceCard();

        if (Bell.Instance.CheckIfClickBell())
        {
            boss.SwitchState(new AttackPhase());
        }
    }

    public override void ExitState(PhaseStateManager boss)
    {
        OnExitDecisionState?.Invoke(this, EventArgs.Empty);
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
