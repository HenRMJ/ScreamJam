using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currentState;
    public State CurrentState { get { return currentState; } }

    public void SwitchState(State newState)
    {
        Debug.Log($"SWITCH STATE FROM {currentState} to {newState}");
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    void Update()
    {
        currentState?.Tick();
    }
}
