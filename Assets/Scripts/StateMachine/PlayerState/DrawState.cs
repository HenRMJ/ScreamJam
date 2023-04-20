using UnityEngine;

/// <summary>
/// Class <c>DrawState</c> represents the game when either player is drawing a card.
/// This state will check the Player object's Hand and Deck to decide if drawing is possible.
/// </summary>
public class DrawState : BasePlayerState
{
    public DrawState(PlayerStateMachine stateMachine, Player player) : base(stateMachine, player) { }

    public override void Enter()
    {
        Debug.Log($"BEGIN DrawState for '{player}'");
    }

    public override void Tick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ClickedDeck())
            {
                Utils.PlayerDrawACard();
                stateMachine.SwitchState(new DecisionState(stateMachine, player));
            }
        }
    }

    public override void Exit()
    {
        Debug.Log($"END DrawState for '{player}'");
    }

    private bool ClickedDeck()
    {
        GameObject clicked = Utils.GetCardObjectUnderCursor();
        if (clicked != null)
        {
            if(clicked.TryGetComponent<CardData>(out CardData cardData))
            {
                return cardData.InDeck;
            }

            return false;
        }
        return false;
    }
}
