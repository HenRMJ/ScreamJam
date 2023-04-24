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

    public static event EventHandler OnPlayerDied;

    public bool CanSummon { get; private set; }

    private int startingBlood;
    [SerializeField] private int blood;
    [SerializeField] private Hand playerHand;
    [SerializeField] private Transform bloodLevel = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("A player already exists in the scene");
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        playerHand.OnCardSummoned += PlayerHand_OnCardSummoned;
        StartRoundState.OnStartRound += StartRoundState_OnStartRound;
        AttackState.OnAttackStateStarted += AttackState_OnAttackStateStarted;
        startingBlood = blood;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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
        StartCoroutine(UpdateBloodLevel());
        return true;
    }

    public void DealDamage(int damage)
    {
        blood -= damage;
        StartCoroutine(UpdateBloodLevel());
    }

    public void Heal(int healAmount)
    {
        blood += healAmount;
        StartCoroutine(UpdateBloodLevel());
    }

    public int GetBlood() => blood;

    private IEnumerator UpdateBloodLevel()
    {
        float t = 0f;
        float speed = 1f;

        Vector3 startingBloodLevel = bloodLevel.localScale;
        float newBloodPercent = (float)blood / (float)startingBlood;
        Vector3 finalBloodLevel = new Vector3(1, newBloodPercent, 1);
        while (t < 1)
        {
            t += Time.deltaTime / speed;
            if (t > 1) { t = 1; }

            bloodLevel.localScale = Vector3.Lerp(startingBloodLevel, finalBloodLevel, t);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
