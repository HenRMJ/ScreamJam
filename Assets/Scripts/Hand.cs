using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public event EventHandler OnCardSelected;
    public event EventHandler OnCardUnselected;

    [SerializeField] private Transform minPosition, maxPosition;
    [SerializeField] private bool belongsToPlayer;
    [SerializeField] private Deck deck;

    public bool BelongsToPlayer { get; private set; }

    private List<Transform> cardsInHand = new List<Transform>();
    private List<Vector3> cardPositions = new List<Vector3>();

    private bool cardIsSelected;
    private Transform selectedCard;    

    private void Awake()
    {
        BelongsToPlayer = belongsToPlayer;
        cardIsSelected = false;
    }

    public void SelectCard()
    {
        GameObject cursorCard = Utils.GetCardObjectUnderCursor();

        if (cursorCard == null) return;
        
        // replace logic with input system and probably makes more sense to have it in the state machine
        if (Input.GetMouseButtonUp(0))
        {
            if (cardIsSelected && cursorCard.transform != selectedCard)
            {
                UnselectCard();
            }

            CardData cardData = cursorCard.GetComponent<CardData>();

            if (cardData.InDeck) return;
            if (cardData.InPlay) return;

            cardIsSelected = true;
            cardData.InHand = false;
            
            selectedCard = cursorCard.transform;
            cardsInHand.Remove(selectedCard);

            Transform selectedTransform = Camera.main.transform.GetChild(0);

            cardData.MoveToPoint(selectedTransform.position, selectedTransform.rotation);
            SetCardPositionsInHand();
            OnCardSelected?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ClickToUnselectCard()
    {
        if (!cardIsSelected) return;

        GameObject cursorCard = Utils.GetCardObjectUnderCursor();

        if (cursorCard == null) return;
        if (cursorCard.transform != selectedCard) return;

        // replace logic in state machine with new input system
        if (Input.GetMouseButtonDown(0))
        {
            UnselectCard();
        }
    }

    private void UnselectCard()
    {
        CardData cardData = selectedCard.GetComponent<CardData>();

        cardData.InHand = true;
        cardIsSelected = false;
        OnCardUnselected?.Invoke(this, EventArgs.Empty);

        ReturnCardToHand(selectedCard);
    }

    public void AddCardToHand()
    {
        Transform cardToAdd = deck.DealCard();

        if (cardToAdd == null) return;

        cardToAdd.parent = transform;
        cardsInHand.Add(cardToAdd);
        SetCardPositionsInHand();
    }

    private void ReturnCardToHand(Transform cardToReturn)
    {
        if (cardToReturn == null)
        {
            Debug.LogWarning("There is no card returning to the hand");
            return;
        }

        cardsInHand.Add(cardToReturn);
        SetCardPositionsInHand();
    }

    private void SetCardPositionsInHand()
    {
        if (cardsInHand.Count == 0) return;
        
        cardPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;

        if (cardsInHand.Count > 1)
        {
            distanceBetweenPoints = (maxPosition.position - minPosition.position) / (cardsInHand.Count - 1);
        }

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CardData cardData = cardsInHand[i].GetComponent<CardData>();

            cardPositions.Add(minPosition.position + (distanceBetweenPoints * i));
            cardData.MoveToPoint(cardPositions[i], minPosition.rotation);
            cardData.PositionInHand = i;
        }
    }

    public void PlaceCard()
    {
        if (!cardIsSelected) return;

        Transform cursorTransform = Utils.GetTransformUnderCursor();

        if (cursorTransform == null) return;
        if (!cursorTransform.TryGetComponent(out CardSlot cardSlot)) return;
        if (cardSlot.Card != null) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            selectedCard.parent = null;

            CardData cardData = selectedCard.GetComponent<CardData>();
            cardData.InHand = false;
            cardData.InDeck = false;
            cardData.InPlay = true;

            cardData.MoveToPoint(cursorTransform.position, cursorTransform.rotation);

            cardSlot.Card = selectedCard;
            selectedCard = null;
            cardIsSelected = false;
            OnCardUnselected?.Invoke(this, EventArgs.Empty);
        }
    }

    public Transform GetSelectedCard()
    {
        if (!cardIsSelected)
        {
            Debug.LogWarning("There is no card selected");
            return null;
        }

        selectedCard.parent = null;
        return selectedCard;
    }
    public Vector3 GetPositionInHand(int i) => cardPositions[i];
    public Quaternion GetRotationInHand() => minPosition.rotation;
}
