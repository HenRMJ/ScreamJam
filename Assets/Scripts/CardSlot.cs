using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public Transform Card { get; set; }
    public bool CanPlace { get; private set; }

    private List<GameObject> validMovePositions = new List<GameObject>();

    private Hand playerHand;
    private GridManager gridManager;

    private Vector2Int cardSlotPosition;

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

        playerHand.OnCardSelected += PlayerHand_OnCardSelected;
        playerHand.OnCardUnselected += PlayerHand_OnCardsUnselected;
    }

    private void Update()
    {
        ShowValidMovePositions();
    }

    private void ShowValidMovePositions()
    {
        if (Card == null) return;
        if (Utils.GetTransformUnderCursor() != transform) return;
        
        if (Input.GetMouseButtonUp(1))
        {
            CardData cardData = Card.GetComponent<CardData>();
            if (!cardData.canMove) return;

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
                Debug.Log(FindObjectsOfType<CardSlot>().Length);
            }

            foreach (GameObject cardSlotObject in validMovePositions)
            {
                cardSlotObject.GetComponent<CardSlot>().UpdateVisuals(true);
            }
        }

        
    }

    private void PlayerHand_OnCardsUnselected(object sender, EventArgs e)
    {
        UpdateVisuals(true);
    }

    private void PlayerHand_OnCardSelected(object hand, EventArgs e)
    {
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

    // can replace with meshrenderer with an animation or material swap
    public void UpdateVisuals(bool updateValue)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = updateValue;
    }
}
