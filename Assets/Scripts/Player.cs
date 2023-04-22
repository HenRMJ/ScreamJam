using System;
using UnityEngine;

/// <summary>
/// Class <c>Player</c> represents each player in the game including
/// their personal attributes (i.e. Health), Deck, Hand and PlayArea
/// </summary>
public class Player : MonoBehaviour
{
    public static event EventHandler OnPlayerDied;

    public bool CanSummon { get; private set; }

    [SerializeField] private int blood;
    [SerializeField] private Hand playerHand;

    // Start is called before the first frame update
    void Start()
    {
        playerHand.OnCardSummoned += PlayerHand_OnCardSummoned;
        StartRoundState.OnStartRound += StartRoundState_OnStartRound;
        AttackState.OnAttackStateStarted += AttackState_OnAttackStateStarted;
    }

    private void AttackState_OnAttackStateStarted(object sender, EventArgs e)
    {
        if (blood <= 0)
        {
            OnPlayerDied?.Invoke(this, EventArgs.Empty);
        }
    }

    private void StartRoundState_OnStartRound(object sender, EventArgs e)
    {
        CanSummon = true;
    }

    private void PlayerHand_OnCardSummoned(object sender, Transform e)
    {
        bool canSummon = true;

        if (e.GetComponent<CardData>().Type == CardType.Monster)
        {
            canSummon = false;
        }

        CanSummon = canSummon;
    }

    public bool TrySummonCard(int cost)
    {
        if (cost >= blood) return false;
        if (cost <= 0) return true;

        blood -= cost;
        return true;
    }

    public void DealDamage(int damage)
    {
        blood -= damage;
    }

    public void Heal(int healAmount)
    {
        blood += healAmount;
    }
}
