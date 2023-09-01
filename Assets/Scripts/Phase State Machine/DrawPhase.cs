using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPhase : PhaseBase
{
    public static event EventHandler OnEnterDrawState;

    private bool switchedStatesBecauseOfCard = false;

    public override void EnterState(PhaseStateManager boss)
    {
        TurnSystem.Instance.IsPlayersTurn = true;
        OnEnterDrawState?.Invoke(this, EventArgs.Empty);
    }

    public override void Tick(PhaseStateManager boss)
    {
        int numberOfCardsInDeck = Player.Instance.GetPlayerHand().GetDeck().GetNumberOfCardsInDeck();

        if (Input.GetMouseButtonDown(0))
        {
            if (ClickedDeck())
            {
                Utils.PlayerDrawACard();
                boss.SwitchState(new MainPhase());
            }
        } else if (numberOfCardsInDeck <= 0 && !switchedStatesBecauseOfCard)
        {
            boss.SwitchState(new MainPhase());
            switchedStatesBecauseOfCard = true;
        }
    }

    public override void ExitState(PhaseStateManager boss)
    {

    }
    private bool ClickedDeck()
    {
        GameObject clicked = Utils.GetCardObjectUnderCursor();

        if (clicked != null)
        {
            if (clicked.TryGetComponent(out CardData cardData))
            {
                if (!cardData.BelongsToPlayer()) return false;

                return cardData.InDeck;
            }

            return false;
        }
        return false;
    }
    
}
