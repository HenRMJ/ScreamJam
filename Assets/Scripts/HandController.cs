using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Transform minPosition, maxPosition;

    private List<Transform> cardsInHand = new List<Transform>();
    private List<Vector3> cardPositions = new List<Vector3>();

    private bool cardIsSelected;
    private Transform selectedCard;

    // assigning this in the start method but probably best to assign it in the inspector when everything is parented under player
    private Deck deck;

    private void Start()
    {
        deck = FindAnyObjectByType<Deck>();
        cardIsSelected = false;
    }

    private void Update()
    {
        // just for testing. This deals the card from the deck to the player's hand but this can be done in a state class on click with the deck
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddCardToHand();
        }

        UnselectCard();
        SelectCard();
    }

    private void SelectCard()
    {
        if (cardIsSelected) return;

        GameObject cursorCard = Utils.GetCardObjectUnderCursor();

        if (cursorCard == null) return;

        // replace logic with input system and probably makes more sense to have it in the state machine
        if (Input.GetMouseButtonUp(0))
        {
            CardData cardData = cursorCard.GetComponent<CardData>();

            cardIsSelected = true;
            cardData.InHand = false;
            
            selectedCard = cursorCard.transform;
            cardsInHand.Remove(selectedCard);

            Transform selectedTransform = Camera.main.transform.GetChild(0);

            cardData.MoveToPoint(selectedTransform.position, selectedTransform.rotation);
            SetCardPositionsInHand();
        }
    }

    private void UnselectCard()
    {
        if (!cardIsSelected) return;

        GameObject cursorCard = Utils.GetCardObjectUnderCursor();

        if (cursorCard == null) return;
        if (cursorCard.transform != selectedCard) return;

        // replace logic in state machine with new input system
        if (Input.GetMouseButtonDown(0))
        {
            CardData cardData = cursorCard.GetComponent<CardData>();

            cardData.InHand = true;
            cardIsSelected = false;

            ReturnCardToHand(selectedCard);
        }
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
