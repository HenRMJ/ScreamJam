using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupPhase : PhaseBase
{
    private float timer;
    private int cardsDrawn;

    private const float DEAL_CARD_TIMER = .5f;
    private const int CARDS_TO_DRAW = 3;

    public override void EnterState(PhaseStateManager boss)
    {
        //Debug.Log("BEGIN GAME SETUP");
        TurnSystem.Instance.IsPlayersTurn = false;
    }

      public override void Tick(PhaseStateManager boss)
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = DEAL_CARD_TIMER;
            Utils.EveryoneDrawACard();
            cardsDrawn++;
        }

        if (cardsDrawn >= CARDS_TO_DRAW)
        {
            boss.SwitchState(new DrawPhase());
        }

    }

    public override void ExitState(PhaseStateManager boss)
    {
        
    }
}
