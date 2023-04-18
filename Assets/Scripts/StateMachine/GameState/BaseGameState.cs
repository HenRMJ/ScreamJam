public abstract class BaseGameState : State
{
    protected GameStateMachine stateMachine;

    public BaseGameState(GameStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
}
