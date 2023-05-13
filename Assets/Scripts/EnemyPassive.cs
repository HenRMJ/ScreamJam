using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyPassive : EnemyBase
{
    CardData weakestPlayerCard = null;

    public override bool TryToSetSelectedCard(out Transform cardToSelect)
    {
        int lowestBloodCost = int.MaxValue;
        int weakestDefense = int.MaxValue;

        weakestPlayerCard = null;
        cardToSelect = null;

        GetPlayerCards();

        foreach (CardData cardData in playerCardsInPlay)
        {
            if (cardData.GetDefenseValue() < weakestDefense)
            {
                Debug.Log($"{cardData.name} on {cardData.currentPosition}");
                if (cardData.currentPosition.y == GridManager.Instance.GetGridDimensions().y - 1) continue;
                weakestDefense = cardData.GetDefenseValue();
                weakestPlayerCard = cardData;
            }
        }
        
        foreach (Transform card in hand.GetCardsInHand())
        {
            CardData cardData = card.GetComponent<CardData>();
            int cardBloodCost = cardData.GetBloodCost();
            int cardAttack = cardData.GetAttackDamage();

            if (cardBloodCost <= lowestBloodCost && 
                cardBloodCost < blood &&
                cardAttack >= weakestDefense)
            {
                cardToSelect = card;
                lowestBloodCost = cardBloodCost;
            }
        }

        if (cardToSelect == null) return false;
        selectedCard = cardToSelect;
        return true;
    }

    public override void TryPlaceCard()
    {
        Vector2Int positionToAttack = new Vector2Int();
        CardSlot slotToPlace = null;

        if (possiblePlacements.Count == 0)
        {
            hand.UnselectCard();
            selectedCard = null;
            return;
        }

        if (weakestPlayerCard != null)
        {
            positionToAttack = weakestPlayerCard.currentPosition;

            foreach (CardSlot cardSlot in possiblePlacements)
            {
                if (cardSlot.GetCardSlotPosition() == positionToAttack + new Vector2Int(0, 1))
                {
                    slotToPlace = cardSlot;
                    break;
                }

                if (cardSlot.GetCardSlotPosition() == positionToAttack + new Vector2Int(0, 2))
                {
                    slotToPlace = cardSlot;
                }

                if (cardSlot.GetCardSlotPosition().x == positionToAttack.x &&
                    slotToPlace == null)
                {
                    slotToPlace = cardSlot;
                }
            }
        }

        if (slotToPlace == null)
        {
            System.Random random = new System.Random();
            int i = random.Next(possiblePlacements.Count);

            slotToPlace = possiblePlacements[i];
        }

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

        cardData.MoveToPoint(slotToPlace.transform.position, slotToPlace.transform.rotation);
        slotToPlace.Card = selectedCard;
        cardData.currentPosition = slotToPlace.GetCardSlotPosition();

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
                    GameObject upperLeftObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1);
                    GameObject upperCenterObject = GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);
                    GameObject upperRightObject = GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1);
                    GameObject rightObject = GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y);
                    GameObject leftObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y);

                    CheckIfSlotIsValid(upperLeftObject, possiblePositions);
                    CheckIfSlotIsValid(upperCenterObject, possiblePositions);
                    CheckIfSlotIsValid(upperRightObject, possiblePositions);
                    CheckIfSlotIsValid(rightObject, possiblePositions);
                    CheckIfSlotIsValid(leftObject, possiblePositions);

                    if (possiblePositions.Count == 0) continue;

                    CardSlot movePosition = null;

                    movePosition = BlockOrRandomPosition(possiblePositions, movePosition);

                    cardData.MoveToPoint(movePosition.transform.position, movePosition.transform.rotation);
                    cardData.CanMove = false;
                    cardData.currentPosition = movePosition.GetCardSlotPosition();

                    movePosition.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
                default:
                    GameObject checkUpperSlotObject = GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);
                    GameObject checkLeftSlotObject = GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y);
                    GameObject checkRightSlotObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y);

                    CheckIfSlotIsValid(checkRightSlotObject, possiblePositions);
                    CheckIfSlotIsValid(checkLeftSlotObject, possiblePositions);
                    CheckIfSlotIsValid(checkUpperSlotObject, possiblePositions);

                    if (possiblePositions.Count == 0) continue;

                    CardSlot slot = null;

                    slot = BlockOrRandomPosition(possiblePositions, slot);

                    cardData.MoveToPoint(slot.transform.position, slot.transform.rotation);
                    cardData.CanMove = false;
                    cardData.currentPosition = slot.GetCardSlotPosition();

                    slot.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
            }
        }
    }

    private static CardSlot BlockOrRandomPosition(List<CardSlot> possiblePositions, CardSlot movePosition)
    {
        foreach (CardSlot cSlot in possiblePositions)
        {
            GameObject slotObject = GridManager.Instance.CardAt(cSlot.GetCardSlotPosition().x, cSlot.GetCardSlotPosition().y - 1);

            if (slotObject != null)
            {
                CardSlot slotToCheck = slotObject.GetComponent<CardSlot>();

                if (slotToCheck.Card == null) continue;
                if (!slotToCheck.CardBelongsToPlayer()) continue;

                movePosition = cSlot;
                break;
            }
        }

        if (movePosition == null)
        {
            System.Random random = new System.Random();
            int i = random.Next(possiblePositions.Count);

            movePosition = possiblePositions[i];
        }

        return movePosition;
    }
}
