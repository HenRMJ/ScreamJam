using System;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public Transform Card { get; set; }

    private Hand playerHand;
    private GridManager gridManager;

    private Vector2Int cardSlotPosition;

    private bool canPlace;

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

    private void PlayerHand_OnCardsUnselected(object sender, EventArgs e)
    {
        UpdateVisualAndCollider(true);
    }

    private void PlayerHand_OnCardSelected(object hand, EventArgs e)
    {
        CardData cardData = playerHand.GetSelectedCard().GetComponent<CardData>();

        canPlace = false;

        switch (cardData.Group)
        {
            case CardGroup.C:
                if (cardSlotPosition.y == 0)
                {
                    canPlace = true;
                }
                break;
            default:
                if (cardSlotPosition.y == 0 || cardSlotPosition.y == 1)
                {
                    canPlace = true;
                }
                break;
        }

        UpdateVisualAndCollider(canPlace);
    }

    // can replace with meshrenderer with an animation or material swap
    private void UpdateVisualAndCollider(bool updateValue)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = updateValue;
        gameObject.GetComponent<BoxCollider>().enabled = updateValue;
    }
}
