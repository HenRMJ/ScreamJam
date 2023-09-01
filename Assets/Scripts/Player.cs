using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// Class <c>Player</c> represents each player in the game including
/// their personal attributes (i.e. Health), Deck, Hand and PlayArea
/// </summary>
public class Player : MonoBehaviour
{
    public static Player Instance;

    public event EventHandler OnPlayerDied;
    public event EventHandler OnPlayerHealthChanged;

    public bool CanSummon { get; private set; }
    public int CardsSummoned { get; private set; }

    [SerializeField] private int blood;
    [SerializeField] private Hand playerHand;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("A player already exists in the scene");
            return;
        }

        Instance = this;
        CardsSummoned = 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        playerHand.OnCardSummoned += PlayerHand_OnCardSummoned;
        DrawPhase.OnEnterDrawState += StartRoundState_OnStartRound;
        PlayArea.Instance.OnAttackFinished += PlayArea_OnAttackFinished;
    }

    private void OnDisable()
    {
        playerHand.OnCardSummoned -= PlayerHand_OnCardSummoned;
        DrawPhase.OnEnterDrawState -= StartRoundState_OnStartRound;
        PlayArea.Instance.OnAttackFinished -= PlayArea_OnAttackFinished;
        StopAllCoroutines();
    }

    private void PlayArea_OnAttackFinished(object sender, EventArgs e)
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
        CardsSummoned++;

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
        OnPlayerHealthChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void DealDamage(int damage)
    {
        blood -= damage;
        OnPlayerHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(int healAmount)
    {
        blood += healAmount;
        OnPlayerHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetBlood() => blood;
    public Hand GetPlayerHand() => playerHand;
}
