using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhase : PhaseBase
{
    public static event EventHandler<Transform> OnEnemySelectedCard;
    public static event EventHandler OnEnterEnemyState;

    private PhaseStateManager stateManager;

    private float timer;
    private bool action1, action2, action3;

    private const float WAIT_TO_MOVE = .3f;
    private const float WAIT_TO_PLACE_CARD = 1.8f;
    private const float WAIT_TO_ATTACK = 3.8f;

    public override void EnterState(PhaseStateManager boss)
    {
        stateManager = boss;
        PlayArea.Instance.OnAttackFinished += PlayArea_OnAttackFinished;
        Utils.EnemyDrawACard();

        action1 = false;
        action2 = false;
        action3 = false;

        OnEnterEnemyState?.Invoke(this, EventArgs.Empty);
    }
    public override void Tick(PhaseStateManager boss)
    {
        timer += Time.deltaTime;

        // Debug.Log(timer);

        if (timer >= WAIT_TO_MOVE)
        {
            if (!action1)
            {
                action1 = true;
                Enemy.Instance.MoveCards();
            }
        }

        if (timer >= WAIT_TO_PLACE_CARD)
        {
            if (!action2)
            {
                action2 = true;

                if (Enemy.Instance.TryToSetSelectedCard(out Transform selectedCard))
                {
                    OnEnemySelectedCard?.Invoke(this, selectedCard);
                    Enemy.Instance.TryPlaceCard();
                }
            }
        }

        if (timer >= WAIT_TO_ATTACK)
        {
            if (!action3)
            {
                action3 = true;
                PlayArea.Instance.EnemyAttacks();
            }
        }
    }

    public override void ExitState(PhaseStateManager boss)
    {
        PlayArea.Instance.OnAttackFinished -= PlayArea_OnAttackFinished;
    }

    private void PlayArea_OnAttackFinished(object sender, EventArgs e)
    {
        stateManager.SwitchState(new DrawPhase());
    }
}