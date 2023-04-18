using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private GameStateMachine gameStateMachine;

    public BaseGameState GameState
    {
        get
        {
            return (BaseGameState)gameStateMachine.CurrentState;
        }
    }

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        gameStateMachine = GetComponent<GameStateMachine>();
    }

    void Start()
    {
        gameStateMachine.SwitchState(new GameSetupState(gameStateMachine));
    }

    public void EndOfTurn(Player player)
    {
        
    }

}
