using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmart : EnemyBase
{
    CardData cardToAttack = null;
    // CardData strongestPlayerCard = null;
    bool useFireSpell = false;

    public override bool TryToSetSelectedCard(out Transform cardToSelect)
    {
        useFireSpell = false;
        int lowestBloodCost = int.MaxValue;
        // int strongestAttack = 0;
        int fieldSize = GridManager.Instance.GetGridDimensions().y - 1;

        // strongestPlayerCard = null;
        cardToAttack = null;
        cardToSelect = null;

        CardData lowestCostCard = null;
        CardData strongestCard = null;

        GetPlayerCards();

        playerCardsInPlay.Sort((CardData a, CardData b) =>
        {
            if (a.GetAttackDamage() > b.GetAttackDamage()) return -1;
            if (a.GetAttackDamage() < b.GetAttackDamage()) return 1;
            return 0;
        });

        foreach (CardData cardData in playerCardsInPlay)
        {
            if (cardToSelect != null)
            {
                Debug.Log($"{cardToSelect.GetComponent<CardData>().GetCardName()} will attack {cardToAttack.GetCardName()}");
                break;
            }

            if (cardData.GetCardName() == "The Demon King")
            {
                useFireSpell = true;
            } else if (cardData.currentPosition.y != fieldSize &&
                cardData.currentPosition.y != fieldSize - 1 &&
                cardData.currentPosition.y != fieldSize - 2)
            {
                continue;
            }

            CardSlot attackingSlot = GridManager.Instance.CardAt(cardData.currentPosition + new Vector2Int(0, 1));

            if (attackingSlot != null)
            {
                if (attackingSlot.Card != null)
                {
                    continue;
                }
            }
            
            if (cardData.currentPosition.y == fieldSize)
            {
                useFireSpell = true;
            }

            foreach (Transform card in hand.GetCardsInHand())
            {
                CardData ourCardData = card.GetComponent<CardData>();

                // Finding the lowest cost monster card and highest damage monster card
                if (ourCardData.Type == CardType.Monster &&
                    ourCardData.GetBloodCost() < blood)
                {
                    if (lowestCostCard == null)
                    {
                        lowestCostCard = ourCardData;
                    }
                    
                    if (strongestCard == null)
                    {
                        strongestCard = ourCardData;
                    }

                    if (strongestCard.GetAttackDamage() <= ourCardData.GetAttackDamage())
                    {
                        strongestCard = ourCardData;
                    }

                    if (lowestCostCard.GetBloodCost() >= ourCardData.GetBloodCost())
                    {
                        lowestCostCard = ourCardData;
                    }

                }

                // if we want to use a firespell and can afford it, we select the fire spell
                if (useFireSpell &&
                    ourCardData.Type == CardType.Spell &&
                    ourCardData.GetBloodCost() < blood)
                {
                    cardToSelect = card;
                    cardToAttack = cardData;
                    break;
                }

                int cardBloodCost = ourCardData.GetBloodCost();
                int cardAttack = ourCardData.GetAttackDamage();
                int cardToAttackDefense = cardData.GetDefenseValue();

                // if we don't use a firespell we look for the lowest cost card that can defeat the strongest enemy
                if (cardBloodCost <= lowestBloodCost &&
                    cardBloodCost < blood &&
                    cardAttack >= cardToAttackDefense)
                {
                    cardToSelect = card;
                    cardToAttack = cardData;
                    lowestBloodCost = cardBloodCost;
                }
            }
        }

        Hand playerHand = Player.Instance.GetPlayerHand();

        // Block to prevent stalemate if we're able to summon a card but the player isn't
        if (cardToSelect == null)
        {
            // if the player has no more cards to draw we continue with the logic
            if (playerHand.GetDeck().GetNumberOfCardsInDeck() <= 0)
            {
                // if the player can't cast a spell we summon our strongest card
                if (!playerHand.CanSummonAMonster() &&
                !playerHand.CanCastASpell())
                {
                    cardToSelect = strongestCard.transform;
                }

                // if the player can cast a spell we summon lowest cost card
                if (!playerHand.CanSummonAMonster() &&
                    playerHand.CanCastASpell())
                {
                    cardToSelect = lowestCostCard.transform;
                }
            } 
        }
        
        if (cardToSelect == null)
        {
            // At this point if our cardToSelect is null than we also can't summon any cards so we only have to check if the player can summon a card and that they have no cards
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
        Vector2Int positionToAttack = new Vector2Int();
        CardSlot slotToPlace = null;
        CardData cardData = selectedCard.GetComponent<CardData>();

        // if there are no places to place our card we return
        if (possiblePlacements.Count == 0)
        {
            hand.UnselectCard();
            selectedCard = null;
            return;
        }

        if (cardData.Type != CardType.Spell)
        {
            useFireSpell = false;
        }

        //If we're not using the fire spell we go through all of this
        if (!useFireSpell)
        {
            // if there is no card to Attack we return
            if (cardToAttack != null)
            {
                positionToAttack = cardToAttack.currentPosition;

                // we're placing the card in a position where we can attack the card
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

            // if there are no available attacking positions, we pick a random position
            if (slotToPlace == null)
            {
                System.Random random = new System.Random();
                int i = random.Next(possiblePlacements.Count);

                slotToPlace = possiblePlacements[i];
            }

            // this is the logic for a monster card to be placed and to change all the properties of the card and cardSlot
            selectedCard.parent = null;

            cardData.InHand = false;
            cardData.InDeck = false;
            cardData.InPlay = true;
            cardData.CanMove = false;

            cardData.MoveToPoint(slotToPlace.transform.position, slotToPlace.transform.rotation);
            slotToPlace.Card = selectedCard;
            cardData.currentPosition = slotToPlace.GetCardSlotPosition();

            hand.GetCardsInHand().Remove(selectedCard);
            CallEnemySummonedCard();

            blood -= cardData.GetBloodCost();
            CallEnemyHealthChanged();
        }
        else
        {
            hand.GetCardsInHand().Remove(selectedCard);
            CallEnemySummonedCard();

            blood -= cardData.GetBloodCost();
            CallEnemyHealthChanged();

            cardData.GetComponent<SpellAction>().CastSpell(GridManager.Instance.CardAt(cardToAttack.currentPosition).GetComponent<CardSlot>());
        }

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
                    CardSlot upperLeftObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1);
                    CardSlot upperCenterObject = GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);
                    CardSlot upperRightObject = GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1);
                    CardSlot rightObject = GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y);
                    CardSlot leftObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y);
                    CardSlot bottomRightObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y + 1);
                    CardSlot bottomLeftObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y + 1);

                    CheckIfSlotIsValid(upperLeftObject, possiblePositions);
                    CheckIfSlotIsValid(upperCenterObject, possiblePositions);
                    CheckIfSlotIsValid(upperRightObject, possiblePositions);
                    CheckIfSlotIsValid(rightObject, possiblePositions);
                    CheckIfSlotIsValid(leftObject, possiblePositions);
                    CheckIfSlotIsValid(bottomRightObject, possiblePositions);
                    CheckIfSlotIsValid(bottomLeftObject, possiblePositions);

                    if (possiblePositions.Count == 0) continue;

                    CardSlot movePosition = null;

                    movePosition = BlockOrRandomPosition(possiblePositions, movePosition, cardData);

                    cardData.MoveToPoint(movePosition.transform.position, movePosition.transform.rotation);
                    cardData.CanMove = false;
                    cardData.currentPosition = movePosition.GetCardSlotPosition();

                    movePosition.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
                default:
                    CardSlot checkUpperSlotObject = GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1);
                    CardSlot checkLeftSlotObject = GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y);
                    CardSlot checkRightSlotObject = GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y);

                    CheckIfSlotIsValid(checkRightSlotObject, possiblePositions);
                    CheckIfSlotIsValid(checkLeftSlotObject, possiblePositions);
                    CheckIfSlotIsValid(checkUpperSlotObject, possiblePositions);

                    if (possiblePositions.Count == 0) continue;

                    CardSlot slot = null;

                    slot = BlockOrRandomPosition(possiblePositions, slot, cardData);

                    cardData.MoveToPoint(slot.transform.position, slot.transform.rotation);
                    cardData.CanMove = false;
                    cardData.currentPosition = slot.GetCardSlotPosition();

                    slot.Card = cardSlot.Card;

                    cardSlot.Card = null;
                    break;
            }
        }
    }

    private static CardSlot BlockOrRandomPosition(List<CardSlot> possiblePositions, CardSlot movePosition, CardData cardData)
    {
        // Attempting to block any opposing cards if we can survive the attack
        foreach (CardSlot cardSlot in possiblePositions)
        {
            CardSlot slotToCheck = GridManager.Instance.CardAt(cardSlot.GetCardSlotPosition().x, cardSlot.GetCardSlotPosition().y - 1);

            if (slotToCheck != null)
            {
                if (slotToCheck.Card == null) continue;
                if (!slotToCheck.CardBelongsToPlayer()) continue;

                CardData playerCard = slotToCheck.Card.GetComponent<CardData>();

                if (cardData.GetDefenseValue() <= playerCard.GetAttackDamage()) continue;

                movePosition = cardSlot;
                break;
            }
        }

        // Attempts to pick a position closer to the enemies side
        if (movePosition == null)
        {
            foreach(CardSlot cardSlot in possiblePositions)
            {
                if (cardSlot.GetCardSlotPosition().y < cardData.currentPosition.y)
                {
                    movePosition = cardSlot;
                }
            }        
        }

        // Picks a random position if no other positions were found
        if (movePosition == null)
        {
            System.Random random = new System.Random();
            int i = random.Next(possiblePositions.Count);

            movePosition = possiblePositions[i];
        }

        return movePosition;
    }
}
