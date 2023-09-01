using UnityEngine;

public class PhaseStateManager : MonoBehaviour
{

    PhaseBase currentState;

    private void Start()
    {
        currentState = new GameSetupPhase();

        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.Tick(this);
    }

    public void SwitchState(PhaseBase stateToSwitchTo)
    {
        currentState.ExitState(this);
        currentState = stateToSwitchTo;
        currentState.EnterState(this);
        Debug.Log(currentState);
    }
}