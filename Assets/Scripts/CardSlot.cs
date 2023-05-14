using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public static event EventHandler OnCanBePlaced;

    public static event EventHandler<bool> OnCardVisualUpdate;

    private static Transform selectedCard;

    public Transform Card { get; set; }
    public bool CanPlace { get; private set; }
    public bool CanMove { get; private set; }

    [SerializeField] private Transform border;
    [SerializeField] private Material mat;
    
    private Animator animator;
    private List<GameObject> validMovePositions = new List<GameObject>();
    private Hand playerHand;
    private Vector2Int cardSlotPosition;
    private bool inDecisionState;

    private void Start()
    {
        animator = GetComponent<Animator>();

        foreach(Hand hand in FindObjectsOfType<Hand>())
        {
            if (hand.BelongsToPlayer)
            {
                playerHand = hand;
            }
        }

        for (int x = 0; x < GridManager.Instance.GetGridDimensions().x; x++)
        {
            for (int y = 0; y < GridManager.Instance.GetGridDimensions().y; y++)
            {
                if (GridManager.Instance.CardAt(x, y) == this)
                {
                    cardSlotPosition = new Vector2Int(x, y);
                }
            }
        }

        inDecisionState = false;

        StartRoundState.OnEnemySelectedCard += StartRoundState_OnEnemySelectedCard;
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

    private void OnDestroy()
    {
        StartRoundState.OnEnemySelectedCard -= StartRoundState_OnEnemySelectedCard;
        playerHand.OnCardSelected -= PlayerHand_OnCardSelected;
        playerHand.OnCardUnselected -= PlayerHand_OnCardsUnselected;
        DecisionState.OnEnterDecisionState -= DecisionState_OnEnterDecisionState;
        DecisionState.OnExitDecisionState -= DecisionState_OnExitDecisionState;
    }

    private void StartRoundState_OnEnemySelectedCard(object sender, Transform e)
    {
        selectedCard = null;
        validMovePositions.Clear();

        CardData cardData = e.GetComponent<CardData>();

        CanPlace = false;

        switch (cardData.Group)
        {
            case CardGroup.C:
                if (cardSlotPosition.y == 3)
                {
                    if (Card == null)
                    {
                        OnCanBePlaced?.Invoke(this, EventArgs.Empty);
                    }
                }
                break;
            default:
                if (cardSlotPosition.y == 3 || cardSlotPosition.y == 2)
                {
                    if (Card == null)
                    {
                        OnCanBePlaced?.Invoke(this, EventArgs.Empty);
                    }
                }
                break;
        }
    }

    private void MoveSelectedCardToValidPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedCard == null) return;
            if (validMovePositions.Count == 0) return;

            Transform cardSlotTransformToMoveTo = Utils.GetTransformUnderCursor();

            if (!validMovePositions.Contains(cardSlotTransformToMoveTo.gameObject)) return;
            if (Card != selectedCard) return;

            Card = null;
            CardSlot cardSlot = cardSlotTransformToMoveTo.GetComponent<CardSlot>();
            cardSlot.Card = selectedCard;
            CardData cardData = cardSlot.Card.GetComponent<CardData>();
            cardData.CanMove = false;
            cardData.MoveToPoint(cardSlotTransformToMoveTo.position, cardSlotTransformToMoveTo.rotation);
            cardData.currentPosition = cardSlot.cardSlotPosition;

            validMovePositions.Clear();
            selectedCard = null;

            foreach (CardSlot cardSlotInScene in GridManager.Instance.GetAllCardSlots())
            {
                cardSlotInScene.UpdateVisuals(false);
            }

            OnCardVisualUpdate?.Invoke(this, false);
        }
    }

    private void ShowValidMovePositions()
    {
        if (Card == null) return;
        
        if (Input.GetMouseButtonUp(1))
        {
            if (Utils.GetTransformUnderCursor() != transform) return;

            CardData cardData = Card.GetComponent<CardData>();

            if (!cardData.BelongsToPlayer()) return;
            if (!cardData.CanMove) return;

            selectedCard = Card;

            if (playerHand.GetCardIsSelected())
            {
                playerHand.UnselectCard();
            }
            
            switch (cardData.Group)
            {
                case CardGroup.A:
                    if (GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y + 1).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y - 1).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y - 1).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y + 1).gameObject);
                    }

                    break;
                default:
                    if (GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x + 1, cardSlotPosition.y).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x - 1, cardSlotPosition.y).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y + 1).gameObject);
                    }

                    if (GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1) != null)
                    {
                        validMovePositions.Add(GridManager.Instance.CardAt(cardSlotPosition.x, cardSlotPosition.y - 1).gameObject);
                    }
                    break;
            }

            foreach (CardSlot cardSlot in GridManager.Instance.GetAllCardSlots())
            {
                cardSlot.UpdateVisuals(false);
                CanMove = false;
            }

            List<GameObject> invalidMovPositionsAfterCheck = new List<GameObject>();

            foreach (GameObject cardSlotObject in validMovePositions)
            {
                CardSlot cardSlot = cardSlotObject.GetComponent<CardSlot>();

                if (cardSlot.Card == null)
                {
                    CanMove = true;
                    cardSlot.UpdateVisuals(true);
                    OnCardVisualUpdate?.Invoke(this, true);
                } else
                {
                    invalidMovPositionsAfterCheck.Add(cardSlotObject);
                    CanMove = false;
                    cardSlot.UpdateVisuals(false);
                }
            }

            foreach (GameObject invalidPosition in invalidMovPositionsAfterCheck)
            {
                validMovePositions.Remove(invalidPosition);
            }
        }        
    }

    private void PlayerHand_OnCardsUnselected(object sender, EventArgs e)
    {
        UpdateVisuals(false);
        OnCardVisualUpdate?.Invoke(this, false);
        validMovePositions.Clear();
        foreach(CardSlot cardSlot in GridManager.Instance.GetAllCardSlots())
        {
            cardSlot.UpdateSacrificeVisual(false);
        }
        // selectedCard = null;
    }

    private void PlayerHand_OnCardSelected(object hand, EventArgs e)
    {
        selectedCard = null;
        validMovePositions.Clear();

        CardData cardData = playerHand.GetSelectedCard().GetComponent<CardData>();

        CanPlace = false;

        if (cardData.Type == CardType.Monster)
        {
            switch (cardData.Group)
            {
                case CardGroup.C:
                    if (cardSlotPosition.y == 0)
                    {
                        if (Card == null)
                        {
                            CanPlace = true;
                        }
                    }
                    break;
                default:
                    if (cardSlotPosition.y == 0 || cardSlotPosition.y == 1)
                    {
                        if (Card == null)
                        {
                            CanPlace = true;
                        }
                    }
                    break;
            }
        } else
        {
            if (Card != null && !CardBelongsToPlayer())
            {
                CanPlace = true;
            }
        }
        

        UpdateVisuals(CanPlace);
        OnCardVisualUpdate?.Invoke(this, true);
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
        for (int i = 0; i < border.childCount; i++)
        {
            border.GetChild(i).GetComponent<MeshRenderer>().enabled = updateValue;
        }
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        inDecisionState = true;
    }

    private void DecisionState_OnExitDecisionState(object sender, EventArgs e)
    {
        inDecisionState = false;
        UpdateVisuals(false);
    }

    public void UpdateSacrificeVisual(bool isMarked)
    {
        animator.SetBool("isMarked", isMarked);
    }
    
    public Vector2Int GetCardSlotPosition() => cardSlotPosition;
}
