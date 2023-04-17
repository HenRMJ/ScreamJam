public abstract class BasePlayerState : State
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    public BasePlayerState(PlayerStateMachine stateMachine, Player player)
    {
        this.stateMachine = stateMachine;
        this.player = player;
    }
}
