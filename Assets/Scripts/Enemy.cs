using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance;

    public static event EventHandler OnEnemyDied;

    [SerializeField] private int blood;
    [SerializeField] private Hand hand;

    private List<CardSlot> possiblePlacements = new List<CardSlot>();
    private Transform selectedCard;

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
    }

    private void CardSlot_OnCanBePlaced(object sender, EventArgs e)
    {
        possiblePlacements.Add(sender as CardSlot);
    }

    private void AttackState_OnAttackStateStarted(object sender, EventArgs e)
    {
        if (blood <= 0)
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

            if (cardBloodCost >= highestBloodCost && cardBloodCost <= Blood)
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
        Debug.Log(possiblePlacements.Count);
        if (possiblePlacements.Count == 0)
        {
            hand.UnselectCard();
            return;
        }

        System.Random random = new System.Random();
        int i = random.Next(possiblePlacements.Count);

        Debug.Log(i);

        CardSlot randomCardSlot = possiblePlacements[i];

        Transform cardToSelect = selectedCard;
        CardData cardData = cardToSelect.GetComponent<CardData>();

        hand.GetCardsInHand().Remove(selectedCard);

        selectedCard.parent = null;

        cardData.InHand = false;
        cardData.InDeck = false;
        cardData.InPlay = true;
        cardData.CanMove = false;

        cardData.MoveToPoint(randomCardSlot.transform.position, randomCardSlot.transform.rotation);
        randomCardSlot.Card = selectedCard;

        hand.SetSelectedCard(null);
        hand.SetIsCardSelected(false);
        possiblePlacements.Clear();
        selectedCard = null;
    }

    public Hand GetHand() => hand;
}
