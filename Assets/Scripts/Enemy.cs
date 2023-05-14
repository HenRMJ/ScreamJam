using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : EnemyBase
{
    public override bool TryToSetSelectedCard(out Transform cardToSelect)
    {
        int highestBloodCost = 0;
        cardToSelect = null;

        foreach (Transform card in hand.GetCardsInHand())
        {
            CardData cardData = card.GetComponent<CardData>();
            int cardBloodCost = cardData.GetBloodCost();

            if (cardBloodCost > highestBloodCost && cardBloodCost < blood)
            {
                cardToSelect = card;
                highestBloodCost = cardBloodCost;
            }
        }

        Hand playerHand = Player.Instance.GetPlayerHand();

        if (cardToSelect == null)
        {
            if (!playerHand.CanSummonAMonster() &&
                !playerHand.CanCastASpell() &&
                playerHand.GetDeck().GetNumberOfCardsInDeck() <= 0 &&
                !GridManager.Instance.ThereAreCardsInPlay())
            {
                DeclareStalemate();
            }

            return false; 
        }
        selectedCard = cardToSelect;
        return true;
    }

    public override void TryPlaceCard()
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
        CallEnemySummonedCard();

        selectedCard.parent = null;

        cardData.InHand = false;
        cardData.InDeck = false;
        cardData.InPlay = true;
        cardData.CanMove = false;

        blood -= cardData.GetBloodCost();
        CallEnemyHealthChanged();

        cardData.MoveToPoint(randomCardSlot.transform.position, randomCardSlot.transform.rotation);
        randomCardSlot.Card = selectedCard;
        cardData.currentPosition = randomCardSlot.GetCardSlotPosition();

        hand.SetSelectedCard(null);
        hand.SetIsCardSelected(false);
        possiblePlacements.Clear();
        selectedCard = null;
    }

    public override void MoveCards()
    {
        foreach (CardSlot cardSlot in GridManager.Instance.GetAllCardSlots())
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
                    CardSlot leftSlot = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1);
                    CardSlot centerSlot = GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);
                    CardSlot rightSlot = GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1);

                    CheckIfSlotIsValid(leftSlot, possiblePositions);
                    CheckIfSlotIsValid(centerSlot, possiblePositions);
                    CheckIfSlotIsValid(rightSlot, possiblePositions);

                    if (possiblePositions.Count == 0) continue;

                    System.Random random = new System.Random();
                    int i = random.Next(possiblePositions.Count);

                    CardSlot movePosition = possiblePositions[i];

                    cardData.MoveToPoint(movePosition.transform.position, movePosition.transform.rotation);
                    cardData.CanMove = false;
                    cardData.currentPosition = movePosition.GetCardSlotPosition();

                    movePosition.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
                default:
                    CardSlot slot = GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);

                    if (slot == null) continue;
                    if (slot.Card != null) continue;

                    cardData.MoveToPoint(slot.transform.position, slot.transform.rotation);
                    cardData.CanMove = false;
                    cardData.currentPosition = slot.GetCardSlotPosition();

                    slot.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
            }
        }
    }
}
