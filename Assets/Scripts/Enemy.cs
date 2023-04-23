using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance;

    public static event EventHandler OnEnemyDied;
    public static event EventHandler<Transform> OnEnemySummonedCard;

    [SerializeField] private int blood;
    [SerializeField] private Hand hand;

    private List<CardSlot> possiblePlacements = new List<CardSlot>();
    private Transform selectedCard;

    [SerializeField] private Transform bloodLevel = null;
    private int startingBlood;

    public int Blood { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("You have another enemy in your scene");
            return;
        }
        Instance = this;

        Blood = blood;
    }

    private void Start()
    {
        AttackState.OnAttackStateStarted += AttackState_OnAttackStateStarted;
        CardSlot.OnCanBePlaced += CardSlot_OnCanBePlaced;
        startingBlood = Blood;
    }

    private void CardSlot_OnCanBePlaced(object sender, EventArgs e)
    {
        possiblePlacements.Add(sender as CardSlot);
    }

    private void AttackState_OnAttackStateStarted(object sender, EventArgs e)
    {
        if (Blood <= 0)
        {
            OnEnemyDied?.Invoke(this, EventArgs.Empty);
        }
    }

    public Transform TryToSetSelectedCard()
    {
        int highestBloodCost = 0;
        Transform cardToSelect = null;

        foreach (Transform card in hand.GetCardsInHand())
        {
            CardData cardData = card.GetComponent<CardData>();
            int cardBloodCost = cardData.GetBloodCost();

            if (cardBloodCost > highestBloodCost && cardBloodCost < Blood)
            {
                cardToSelect = card;
                highestBloodCost = cardBloodCost;
            }
        }

        if (cardToSelect == null) return null;
        selectedCard = cardToSelect;
        return cardToSelect;
    }

    public void TryPlaceCard()
    {
        if (possiblePlacements.Count == 0)
        {
            hand.UnselectCard();
            selectedCard = null;
            return;
        }

        System.Random random = new System.Random();
        int i = random.Next(possiblePlacements.Count);

        CardSlot randomCardSlot = possiblePlacements[i];

        Transform cardToSelect = selectedCard;
        CardData cardData = cardToSelect.GetComponent<CardData>();

        hand.GetCardsInHand().Remove(selectedCard);
        OnEnemySummonedCard?.Invoke(this, selectedCard);

        selectedCard.parent = null;

        cardData.InHand = false;
        cardData.InDeck = false;
        cardData.InPlay = true;
        cardData.CanMove = false;

        Blood -= cardData.GetBloodCost();
        StartCoroutine(UpdateBloodLevel());

        cardData.MoveToPoint(randomCardSlot.transform.position, randomCardSlot.transform.rotation);
        randomCardSlot.Card = selectedCard;

        hand.SetSelectedCard(null);
        hand.SetIsCardSelected(false);
        possiblePlacements.Clear();
        selectedCard = null;
    }

    public void MoveCardsForward()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();

        foreach (CardSlot cardSlot in FindObjectsOfType<CardSlot>())
        {
            if (cardSlot.Card == null) continue;
            if (cardSlot.CardBelongsToPlayer()) continue;

            CardData cardData = cardSlot.Card.GetComponent<CardData>();

            if (!cardData.CanMove) continue;

            Vector2Int cardSlotPosition = cardSlot.GetCardSlotPosition();
            List<CardSlot> possiblePositions = new List<CardSlot>();

            switch (cardData.Group)
            {
                case CardGroup.A:
                    GameObject leftObject = gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1);

                    if (leftObject != null)
                    {
                        CardSlot leftSlot = leftObject.GetComponent<CardSlot>();

                        if (leftSlot.Card == null)
                        {
                            possiblePositions.Add(leftSlot);
                        }
                    }

                    GameObject centerObject = gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);

                    if (centerObject != null)
                    {
                        CardSlot centerSlot = centerObject.GetComponent<CardSlot>();

                        if (centerSlot.Card == null)
                        {
                            possiblePositions.Add(centerSlot);
                        }
                    }

                    GameObject rightObject = gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1);

                    if (rightObject != null)
                    {
                        CardSlot rightSlot = rightObject.GetComponent<CardSlot>();

                        if (rightSlot.Card == null)
                        {
                            possiblePositions.Add(rightSlot);
                        }
                    }

                    if (possiblePositions.Count == 0) continue;

                    System.Random random = new System.Random();
                    int i = random.Next(possiblePositions.Count);

                    CardSlot movePosition = possiblePositions[i];

                    cardData.MoveToPoint(movePosition.transform.position, movePosition.transform.rotation);
                    cardData.CanMove = false;

                    movePosition.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
                default:
                    GameObject checkSlotObject = gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);

                    if (checkSlotObject == null) continue;

                    CardSlot slot = checkSlotObject.GetComponent<CardSlot>();

                    if (slot.Card != null) continue;

                    cardData.MoveToPoint(slot.transform.position, slot.transform.rotation);
                    cardData.CanMove = false;

                    slot.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
            }
        }
    }

    public void Heal(int healAmount)
    {
        Blood += healAmount;
        StartCoroutine(UpdateBloodLevel());
    }
    private IEnumerator UpdateBloodLevel()
    {
        float t = 0f;
        float speed = 1f;

        Vector3 startingBloodLevel = bloodLevel.localScale;
        float newBloodPercent = (float)Blood / (float)startingBlood;
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

    public Hand GetHand() => hand;
}
