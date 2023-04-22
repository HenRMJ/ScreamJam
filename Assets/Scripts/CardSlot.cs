using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    private static Transform selectedCard;

    public Transform Card { get; set; }
    public bool CanPlace { get; private set; }
    public bool CanMove { get; private set; }

    private List<GameObject> validMovePositions = new List<GameObject>();

    private Hand playerHand;
    private GridManager gridManager;

    private Vector2Int cardSlotPosition;
    private bool inDecisionState;

    private void Start()
    {
        foreach(Hand hand in FindObjectsOfType<Hand>())
        {
            if (hand.BelongsToPlayer)
            {
                playerHand = hand;
            }
        }

        gridManager = FindObjectOfType<GridManager>();

        for (int x = 0; x < gridManager.GetGridDimensions().x; x++)
        {
            for (int y = 0; y < gridManager.GetGridDimensions().y; y++)
            {
                if (gridManager.CardAt(x, y) == gameObject)
                {
                    cardSlotPosition = new Vector2Int(x, y);
                }
            }
        }

        inDecisionState = false;

        playerHand.OnCardSelected += PlayerHand_OnCardSelected;
        playerHand.OnCardUnselected += PlayerHand_OnCardsUnselected;
        DecisionState.OnEnterDecisionState += DecisionState_OnEnterDecisionState;
        DecisionState.OnExitDecisionState += DecisionState_OnExitDecisionState;
    }

    private void Update()
    {
        if (!inDecisionState) return;
        ShowValidMovePositions();
        MoveSelectedCardToValidPosition();
    }

    private void MoveSelectedCardToValidPosition()
    {
        if (selectedCard == null) return;
        if (validMovePositions.Count == 0) return;

        Transform cardSlotTransformToMoveTo = Utils.GetTransformUnderCursor();

        if (!validMovePositions.Contains(cardSlotTransformToMoveTo.gameObject)) return;
        if (Card != selectedCard) return;

        if (Input.GetMouseButtonDown(0))
        {
            Card = null;
            CardSlot cardSlot = cardSlotTransformToMoveTo.GetComponent<CardSlot>();
            cardSlot.Card = selectedCard;
            CardData cardData = cardSlot.Card.GetComponent<CardData>();
            cardData.CanMove = false;
            cardData.MoveToPoint(cardSlotTransformToMoveTo.position, cardSlotTransformToMoveTo.rotation);

            validMovePositions.Clear();
            selectedCard = null;
            foreach (CardSlot cardSlotInScene in FindObjectsOfType<CardSlot>())
            {
                cardSlotInScene.UpdateVisuals(true);
            }
        }
    }

    private void ShowValidMovePositions()
    {
        if (Card == null) return;
        if (Utils.GetTransformUnderCursor() != transform) return;
        
        if (Input.GetMouseButtonUp(1))
        {
            CardData cardData = Card.GetComponent<CardData>();

            if (!cardData.CanMove) return;

            selectedCard = Card;

            switch (cardData.Group)
            {
                case CardGroup.A:
                    if (gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y + 1));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y + 1));
                    }

                    break;
                default:
                    if (gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1));
                    }

                    if (gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(gridManager.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1));
                    }
                    break;
            }

            foreach (CardSlot cardSlot in FindObjectsOfType<CardSlot>())
            {
                cardSlot.UpdateVisuals(false);
                CanMove = false;
                Debug.Log(FindObjectsOfType<CardSlot>().Length);
            }

            foreach (GameObject cardSlotObject in validMovePositions)
            {
                CardSlot cardSlot = cardSlotObject.GetComponent<CardSlot>();

                if (cardSlot.Card == null)
                {
                    CanMove = true;
                    cardSlot.UpdateVisuals(true);
                } else
                {
                    CanMove = false;
                    cardSlot.UpdateVisuals(false);
                }
            }
        }        
    }

    private void PlayerHand_OnCardsUnselected(object sender, EventArgs e)
    {
        UpdateVisuals(true);
        validMovePositions.Clear();
        selectedCard = null;
    }

    private void PlayerHand_OnCardSelected(object hand, EventArgs e)
    {
        selectedCard = null;
        validMovePositions.Clear();

        CardData cardData = playerHand.GetSelectedCard().GetComponent<CardData>();

        CanPlace = false;

        switch (cardData.Group)
        {
            case CardGroup.C:
                if (cardSlotPosition.y == 0)
                {
                    CanPlace = true;
                }
                break;
            default:
                if (cardSlotPosition.y == 0 || cardSlotPosition.y == 1)
                {
                    CanPlace = true;
                }
                break;
        }

        UpdateVisuals(CanPlace);
    }

    public bool CardBelongsToPlayer()
    {
        if (Card == null)
        {
            Debug.LogWarning("If this is occuring, you are not checking if the card exists. Check if card exists before calling this method");
            return false;
        }

        return Card.GetComponent<CardData>().BelongsToPlayer();
    }

    // can replace with meshrenderer with an animation or material swap
    public void UpdateVisuals(bool updateValue)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = updateValue;
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        inDecisionState = true;
    }

    private void DecisionState_OnExitDecisionState(object sender, EventArgs e)
    {
        inDecisionState = false;
    }

    public Vector2Int GetCardSlotPosition() => cardSlotPosition;
}
