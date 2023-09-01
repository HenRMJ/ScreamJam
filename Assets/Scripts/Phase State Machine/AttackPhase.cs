using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPhase : PhaseBase
{
    public static event EventHandler OnAttackStateStarted;

    private PhaseStateManager stateManager;

    public override void EnterState(PhaseStateManager boss)
    {
        PlayArea.Instance.OnAttackFinished += PlayArea_OnAttackFinished;
        OnAttackStateStarted?.Invoke(this, EventArgs.Empty);

        stateManager = boss;
    }

    public override void Tick(PhaseStateManager boss)
    {

    }

    public override void ExitState(PhaseStateManager boss)
    {
        PlayArea.Instance.OnAttackFinished -= PlayArea_OnAttackFinished;
    }

    private void PlayArea_OnAttackFinished(object sender, EventArgs e)
    {
        stateManager.SwitchState(new PostPhase());
    }
}
    
