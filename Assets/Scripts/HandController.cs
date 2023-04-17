using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private List<Transform> cardsInHand;
    [SerializeField] private Transform minPosition, maxPosition;
    
    private List<Vector3> cardPositions = new List<Vector3>();

    // assigning this in the start method but probably best to assign it in the inspector when everything is parented under player
    private Deck deck;

    private void Start()
    {
        deck = FindAnyObjectByType<Deck>();
    }

    private void Update()
    {
        // just for testing. This deals the card from the deck to the player's hand but this can be done in a state class on click with the deck
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddCardToHand();
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
            cardPositions.Add(minPosition.position + (distanceBetweenPoints * i));
            cardsInHand[i].GetComponent<CardData>().MoveToPoint(cardPositions[i], minPosition.rotation);
        }
    }
}
