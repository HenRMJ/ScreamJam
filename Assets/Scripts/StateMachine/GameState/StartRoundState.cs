using UnityEngine;
using System;

public class StartRoundState : BaseGameState
{
    public static event EventHandler OnStartRound;

    public StartRoundState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("BEGIN StartRoundState");

        TurnSystem.Instance.AttackedThisRound = false;

        // Find the player object, set its state to the first state in the game loop
        // TODO finding the player object like this is just a temprorary hack and should be replaced
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerStateMachine playerStateMachine = playerObject.GetComponent<PlayerStateMachine>();
        Player player = playerObject.GetComponent<Player>();
        playerStateMachine.SwitchState(new DrawState(playerStateMachine, player));

        TurnSystem.Instance.IsPlayersTurn = !TurnSystem.Instance.IsPlayersTurn;

        OnStartRound?.Invoke(this, EventArgs.Empty);
    }

    public override void Tick()
    {
        if (TurnSystem.Instance.IsPlayersTurn)
        {
            if (Bell.Instance.CheckIfClickBell())
            {
                stateMachine.SwitchState(new AttackState(stateMachine));
            }
        } else
        {
            stateMachine.SwitchState(new StartRoundState(stateMachine));
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            PlayerStateMachine playerStateMachine = playerObject.GetComponent<PlayerStateMachine>();
            Player player = playerObject.GetComponent<Player>();
            playerStateMachine.SwitchState(new DrawState(playerStateMachine, player));
        }
        
    }

    public override void Exit()
    {
        Debug.Log("END StartRoundState");
    }
}

