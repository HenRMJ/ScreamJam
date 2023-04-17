using UnityEngine;

public class DecisionState : BasePlayerState
{
    public DecisionState(PlayerStateMachine stateMachine, Player player) : base(stateMachine, player) { }

    public override void Enter()
    {
        Debug.Log($"BEGIN DecisionState for '{player}'");
    }


    public override void Tick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ClickedHand())
            {
                Debug.Log("HAND CLICKED, INFORM GAMEMANAGER OF END OF TURN");
            }
        }
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
}

