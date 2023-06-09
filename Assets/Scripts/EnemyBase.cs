using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EnemyBase : MonoBehaviour
{
    public static EnemyBase Instance;

    public event EventHandler OnEnemyHealthChanged;
    public event EventHandler OnEnemyDied;
    public event EventHandler<Transform> OnEnemySummonedCard;
    public event EventHandler OnStalemate;

    public int CardsSummoned { get; private set; }

    [SerializeField] protected int blood;
    [SerializeField] protected Hand hand;

    protected List<CardSlot> possiblePlacements = new List<CardSlot>();
    protected Transform selectedCard;
    protected List<CardData> playerCardsInPlay = new List<CardData>();

    protected void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("You have another enemy in your scene: " + transform);
            return;
        }

        Instance = this;
        CardsSummoned = 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        PlayArea.Instance.OnAttackFinished += PlayArea_OnAttackFinished;
        CardSlot.OnCanBePlaced += CardSlot_OnCanBePlaced;
    }

    private void OnDisable()
    {
        PlayArea.Instance.OnAttackFinished -= PlayArea_OnAttackFinished;
        CardSlot.OnCanBePlaced -= CardSlot_OnCanBePlaced;
        StopAllCoroutines();
    }

    public abstract bool TryToSetSelectedCard(out Transform cardToSelect);
    public abstract void MoveCards();
    public abstract void TryPlaceCard();

    private void CardSlot_OnCanBePlaced(object sender, EventArgs e)
    {
        possiblePlacements.Add(sender as CardSlot);
    }

    private void PlayArea_OnAttackFinished(object sender, EventArgs e)
    {
        if (blood <= 0)
        {
            OnEnemyDied?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnEnemyHealthChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected void CallEnemyHealthChanged()
    {
        OnEnemyHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void CallEnemySummonedCard()
    {
        CardsSummoned++;
        OnEnemySummonedCard?.Invoke(this, selectedCard);
    }

    protected void DeclareStalemate()
    {
        Debug.Log("Stalemate");
        OnStalemate?.Invoke(this, EventArgs.Empty);
    }

    public void SetBlood(int amountToChange)
    {
        blood += amountToChange;
        OnEnemyHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void GetPlayerCards()
    {
        playerCardsInPlay.Clear();

        foreach (CardSlot cardSlot in GridManager.Instance.GetAllCardSlots())
        {
            if (cardSlot.Card == null) continue;
            if (!cardSlot.CardBelongsToPlayer()) continue;

            CardData playerCardData = cardSlot.Card.GetComponent<CardData>();

            playerCardsInPlay.Add(playerCardData);
        }
    }

    protected void CheckIfSlotIsValid(CardSlot slot, List<CardSlot> possibleSlots)
    {
        if (slot != null)
        {
            if (slot.Card == null)
            {
                possibleSlots.Add(slot);
            }
        }
    }

    public int GetBlood() => blood;
    public Hand GetHand() => hand;
}
