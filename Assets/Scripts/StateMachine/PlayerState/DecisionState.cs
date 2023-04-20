using UnityEngine;

public class DecisionState : BasePlayerState
{
    public DecisionState(PlayerStateMachine stateMachine, Player player) : base(stateMachine, player) { }

    Hand playerHand;

    public override void Enter()
    {
        Debug.Log($"BEGIN DecisionState for '{player}'");
        AssignPlayerHand();
    }


    public override void Tick()
    {
        playerHand.ClickToUnselectCard();
        playerHand.SelectCard();
        playerHand.PlaceCard();
    }

    public override void Exit()
    {
        Debug.Log($"BEGIN DecisionState for '{player}'");
    }

    private bool ClickedHand()
    {
        GameObject clicked = Utils.GetCardObjectUnderCursor();
        if (clicked != null)
        {
            return clicked.transform.parent.name == "Hand";
        }
        return false;
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

