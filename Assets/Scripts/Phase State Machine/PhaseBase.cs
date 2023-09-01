using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhaseBase
{
    public abstract void EnterState(PhaseStateManager boss);

    public abstract void Tick(PhaseStateManager boss);

    public abstract void ExitState(PhaseStateManager boss);
}
