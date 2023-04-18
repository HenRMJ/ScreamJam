using System.Collections;
using UnityEngine;

/// <summary>
/// Class <c>GameSetupState</c> is the first state of the game after any initial splash screens or intros.
/// This state can be used to set up the UI, play any sounds or animation, etc. prior to Round 1 beginning.
/// </summary>
public class GameSetupState : BaseGameState
{
    public GameSetupState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("BEGIN GAME SETUP");
        stateMachine.StartCoroutine(SleepAndStartGame());
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {
        Debug.Log("END GAME SETUP");
    }

    public IEnumerator SleepAndStartGame()
    {
        yield return new WaitForSeconds(5f);
        stateMachine.SwitchState(new StartRoundState(stateMachine));
    }
}

