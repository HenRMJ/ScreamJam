using UnityEngine;

public abstract class SpellAction : MonoBehaviour
{
    public abstract void CastSpell(CardSlot cardSlot);
}