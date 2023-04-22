using UnityEngine;
using System;
using System.Collections;

public class StartRoundState : BaseGameState
{
    public static event EventHandler OnStartRound;
    public static event EventHandler<Transform> OnEnemySelectedCard;

    public StartRoundState(GameStateMachine stateMachine) : base(stateMachine) { }

    private bool isEnemysTurn = false;


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
        }
        else if (!isEnemysTurn)
        {
            isEnemysTurn = true;
            stateMachine.StartCoroutine(EnemyActions());
        }

    }

    private IEnumerator EnemyActions()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerStateMachine playerStateMachine = playerObject.GetComponent<PlayerStateMachine>();
        Player player = playerObject.GetComponent<Player>();

        Utils.EnemyDrawACard();

        Debug.Log("Enemy draw, wait for move cards");
        yield return new WaitForSeconds(.4f);

        Enemy.Instance.MoveCardsForward();
        Debug.Log("Enemy moved, wait for TryToSelect");
        yield return new WaitForSeconds(1.5f);

        Transform selectedCard = Enemy.Instance.TryToSetSelectedCard();
        Debug.Log("Enemy selected, wait for attack");
        if (selectedCard != null)
        {
            OnEnemySelectedCard?.Invoke(this, selectedCard);
            Enemy.Instance.TryPlaceCard();
        }

        yield return new WaitForSeconds(3f);
        PlayArea.Instance.AllCardsAttack(false);
        Debug.Log("enemey done");

        stateMachine.SwitchState(new StartRoundState(stateMachine));
        playerStateMachine.SwitchState(new DrawState(playerStateMachine, player));

        isEnemysTurn = false;
        yield return null;
    }

    public override void Exit()
    {
        Debug.Log("END StartRoundState");
    }
}

