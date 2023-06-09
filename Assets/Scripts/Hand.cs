using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public event EventHandler OnCardSelected;
    public event EventHandler OnCardUnselected;
    public event EventHandler<Transform> OnCardSummoned;
    public event EventHandler<string> OnCardNotSummoned;

    [SerializeField] private Transform minPosition, maxPosition;
    [SerializeField] private bool belongsToPlayer;
    [SerializeField] private Deck deck;
    [SerializeField] private Player player;

    public bool BelongsToPlayer { get; private set; }

    private List<Transform> cardsInHand = new List<Transform>();
    private List<Vector3> cardPositions = new List<Vector3>();
    private List<GameObject> cardsToSacrifice = new List<GameObject>();

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
                if (cursorCard.GetComponent<CardData>().InPlay) return;
                UnselectCard();
            }

            CardData cardData = cursorCard.GetComponent<CardData>();

            if (cardData.InDeck) return;
            if (cardData.InPlay) return;
            if (!cardData.BelongsToPlayer()) return;

            AkSoundEngine.PostEvent("SelectCard", gameObject);

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
            if (cursorCard.GetComponent<CardData>().InPlay) return;
            UnselectCard();
        }
    }

    public void UnselectCard()
    {
        CardData cardData = selectedCard.GetComponent<CardData>();

        cardData.InHand = true;
        cardIsSelected = false;

        OnCardUnselected?.Invoke(this, EventArgs.Empty);
        cardsToSacrifice.Clear();

        ReturnCardToHand(selectedCard);
    }

    public void AddCardToHand()
    {
        Transform cardToAdd = deck.DealCard(this);

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
        
        if (Input.GetMouseButtonDown(0))
        {
            Transform cursorTransform = Utils.GetTransformUnderCursor();

            if (cursorTransform == null) return;
            if (!cursorTransform.TryGetComponent(out CardSlot cardSlot)) return;

            CardData cardData = selectedCard.GetComponent<CardData>();

            if (cardData.Type == CardType.Monster && !player.CanSummon)
            {
                Debug.Log("You already summoned a monster this turn");
                OnCardNotSummoned?.Invoke(this, "You already summoned a monster this turn");
                return;
            }

            if (cardSlot.Card != null && cardSlot.CardBelongsToPlayer())
            {
                UpdateCardsToSacrifice();
                return;
            }

            if (!cardSlot.CanPlace) return;

            // This checks if we can summon a card with our sacrifices, and if we can it updates the card slots
            // and it updates the player health, and destorys the cards, if not it just doesn't do it
            if (player.TrySummonCard(cardData.GetBloodCost() - CalculateSacrificedBlood()))
            {

                foreach (CardSlot cardSlotToUpdate in GridManager.Instance.GetAllCardSlots())
                {
                    if (cardSlotToUpdate.Card == null) continue;

                    if (cardsToSacrifice.Contains(cardSlotToUpdate.Card.gameObject))
                    {
                        cardSlotToUpdate.UpdateSacrificeVisual(false, true);
                        cardSlotToUpdate.Card = null;
                    }
                }

                foreach (GameObject card in cardsToSacrifice)
                {
                    AkSoundEngine.PostEvent("SacrificeMonster", gameObject);
                    Destroy(card);
                }

                cardsToSacrifice.Clear();
            } else
            {
                Debug.Log("Not enough blood to summon card");
                OnCardNotSummoned?.Invoke(this, "Not enough blood to summon card");
                return;
            }

            OnCardSummoned?.Invoke(this, selectedCard);

            if (cardData.Type == CardType.Monster)
            {
                selectedCard.parent = null;

                cardData.InHand = false;
                cardData.InDeck = false;
                cardData.InPlay = true;
                cardData.CanMove = false;

                cardData.MoveToPoint(cursorTransform.position, cursorTransform.rotation);
                cardData.currentPosition = cardSlot.GetCardSlotPosition();

                cardSlot.Card = selectedCard;
            } else
            {
                
                cardData.GetComponent<SpellAction>().CastSpell(cardSlot);
            }       

            selectedCard = null;
            cardIsSelected = false;
            
            OnCardUnselected?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UpdateCardsToSacrifice()
    {
        if (!cardIsSelected) return;

        GameObject cursorCard = Utils.GetCardObjectUnderCursor();
        
        if (cursorCard == null) return;

        CardData cardData = cursorCard.GetComponent<CardData>();

        if (!cardData.InPlay) return;
        if (!cardData.BelongsToPlayer()) return;

        CardSlot cardSlot = Utils.GetTransformUnderCursor().GetComponent<CardSlot>();

        if (cardsToSacrifice.Contains(cursorCard))
        {
            cardData.MarkedAsSacrifice = false;
            cardsToSacrifice.Remove(cursorCard);
            cardSlot.UpdateSacrificeVisual(false);
        }
        else
        {
            cardData.MarkedAsSacrifice = true;
            cardsToSacrifice.Add(cursorCard);
            cardSlot.UpdateSacrificeVisual(true);
        }
    }

    private int CalculateSacrificedBlood()
    {
        int sacrificeAmount = 0;

        foreach (GameObject card in cardsToSacrifice)
        {
            sacrificeAmount += card.GetComponent<CardData>().GetBloodCost();
        }

        return sacrificeAmount;
    }

    public Transform GetSelectedCard()
    {
        if (!cardIsSelected)
        {
            // Debug.LogWarning("There is no card selected");
            return null;
        }

        selectedCard.parent = null;
        return selectedCard;
    }

    public void SetIsCardSelected(bool isCardSelected)
    {
        cardIsSelected = isCardSelected;
    }

    public void SetSelectedCard(Transform card)
    {
        //Debug.Log(card);
        selectedCard = card;
    }

    public bool CanSummonAMonster()
    {
        bool canSummon = false;

        foreach (Transform card in cardsInHand)
        {
            CardData cardData = card.GetComponent<CardData>();

            if (cardData.Type != CardType.Monster) continue;

            if (cardData.GetBloodCost() < Player.Instance.GetBlood())
            {
                canSummon = true;
                break;
            }
        }

        return canSummon;
    }

    public bool CanCastASpell()
    {
        bool canSummon = false;

        foreach (Transform card in cardsInHand)
        {
            CardData cardData = card.GetComponent<CardData>();

            if (cardData.Type != CardType.Spell) continue;

            if (cardData.GetBloodCost() < Player.Instance.GetBlood())
            {
                canSummon = true; 
                break;
            }
        }

        return canSummon;
    }

    public Deck GetDeck() => deck;
    public bool GetCardIsSelected() => cardIsSelected;
    public List<Transform> GetCardsInHand() => cardsInHand;
    public Vector3 GetPositionInHand(int i) => cardPositions[i];
    public Quaternion GetRotationInHand() => minPosition.rotation;
}
