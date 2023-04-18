using UnityEngine;

/// <summary>
/// Class <c>WaitingState</c> is occurs when the player is waiting for the opponent to complete their turn.
/// This state can be used by the AI to taunt the player or by the player to inspect the game world.
/// </summary>
public class WaitingState : BasePlayerState
{
    public WaitingState(PlayerStateMachine stateMachine, Player player) : base(stateMachine, player) { }

    public override void Enter()
    {
        Debug.Log($"BEGIN WaitingState for '{player}'");
    }

    public override void Tick()
    {
    }

    public override void Exit()
    {
        Debug.Log($"END WaitingState for '{player}'");
    }
}
