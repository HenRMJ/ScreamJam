using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public static PlayArea Instance;

    public event EventHandler OnAttackFinished;

    [SerializeField] private int avatarMultipilier = 10;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one play area in your scene");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        AttackState.OnAttackStateStarted += AttackState_OnAttackStateStarted;
    }

    private void OnDisable()
    {
        AttackState.OnAttackStateStarted -= AttackState_OnAttackStateStarted;
    }

    private void AttackState_OnAttackStateStarted(object sender, EventArgs e)
    {
        AllCardsAttack(TurnSystem.Instance.IsPlayersTurn);
    }

    public void AllCardsAttack(bool playerAttacking)
    {
        GridManager gridManager = FindObjectOfType<GridManager>();

        foreach (CardSlot cardSlot in GridManager.Instance.GetAllCardSlots())
        {
            if (cardSlot.Card == null) continue;

            if (cardSlot.CardBelongsToPlayer() && playerAttacking)
            {
                CardData cardData = cardSlot.Card.GetComponent<CardData>();

                switch (cardData.Group)
                {
                    case CardGroup.C:
                        Vector2Int currentPosition = cardSlot.GetCardSlotPosition();
                        GameObject attackingPosition = gridManager.CardAt(currentPosition.x, currentPosition.y + 1);

                        if (attackingPosition == null)
                        {
                            
                            Enemy.Instance.Blood -= cardData.GetAttackDamage() * avatarMultipilier;
                            continue;
                        }

                        CardSlot slotToAttack = attackingPosition.GetComponent<CardSlot>();

                        if (slotToAttack.Card == null)
                        {
                            GameObject attackingSecondPosition = gridManager.CardAt(currentPosition.x, currentPosition.y + 2);

                            if (attackingSecondPosition == null)
                            {
                                Enemy.Instance.Blood -= cardData.GetAttackDamage() * avatarMultipilier;
                                continue;
                            }

                            CardSlot secondSlotToAttack = attackingSecondPosition.GetComponent<CardSlot>();

                            if (secondSlotToAttack.Card == null) continue;
                            if (secondSlotToAttack.CardBelongsToPlayer()) continue;

                            CardData secondDefendingCard = secondSlotToAttack.Card.GetComponent<CardData>();

                            if (cardData.GetAttackDamage() >= secondDefendingCard.GetDefenseValue())
                            {
                                secondSlotToAttack.Card = null;
                                AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                                Destroy(secondDefendingCard.gameObject);
                            }
                        }

                        if (slotToAttack.Card == null) continue;
                        if (slotToAttack.CardBelongsToPlayer()) continue;

                        CardData defendingCard = slotToAttack.Card.GetComponent<CardData>();

                        if (cardData.GetAttackDamage() >= defendingCard.GetDefenseValue())
                        {
                            slotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defendingCard.gameObject);
                        }

                        break;
                    default:
                        Vector2Int defaultCurrentPosition = cardSlot.GetCardSlotPosition();
                        GameObject defaultAttackingPosition = gridManager.CardAt(defaultCurrentPosition.x, defaultCurrentPosition.y + 1);

                        if (defaultAttackingPosition == null)
                        {
                            Enemy.Instance.Blood -= cardData.GetAttackDamage() * avatarMultipilier;
                            continue;
                        }

                        CardSlot defaultSlotToAttack = defaultAttackingPosition.GetComponent<CardSlot>();

                        if (defaultSlotToAttack.Card == null) continue;
                        if (defaultSlotToAttack.CardBelongsToPlayer()) continue;

                        CardData defaultDefendingCard = defaultSlotToAttack.Card.GetComponent<CardData>();

                        Debug.Log(defaultDefendingCard);
                        if (cardData.GetAttackDamage() >= defaultDefendingCard.GetDefenseValue())
                        {
                            defaultSlotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defaultDefendingCard.gameObject);
                        }

                        break;
                }
            }

            if (!cardSlot.CardBelongsToPlayer() && !playerAttacking)
            {
                CardData cardData = cardSlot.Card.GetComponent<CardData>();

                switch (cardData.Group)
                {
                    case CardGroup.C:
                        Vector2Int currentPosition = cardSlot.GetCardSlotPosition();
                        GameObject attackingPosition = gridManager.CardAt(currentPosition.x, currentPosition.y - 1);

                        if (attackingPosition == null)
                        {
                            Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                            continue;
                        }

                        CardSlot slotToAttack = attackingPosition.GetComponent<CardSlot>();

                        if (slotToAttack.Card == null)
                        {
                            GameObject attackingSecondPosition = gridManager.CardAt(currentPosition.x, currentPosition.y - 2);

                            if (attackingSecondPosition == null)
                            {
                                Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                                continue;
                            }

                            CardSlot secondSlotToAttack = attackingSecondPosition.GetComponent<CardSlot>();

                            if (secondSlotToAttack.Card == null) continue;
                            if (!secondSlotToAttack.CardBelongsToPlayer()) continue;

                            CardData secondDefendingCard = secondSlotToAttack.Card.GetComponent<CardData>();

                            if (cardData.GetAttackDamage() >= secondDefendingCard.GetDefenseValue())
                            {
                                secondSlotToAttack.Card = null;
                                AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                                Destroy(secondDefendingCard.gameObject);
                            }
                        }

                        if (slotToAttack.Card == null) continue;
                        if (!slotToAttack.CardBelongsToPlayer()) continue;

                        CardData defendingCard = slotToAttack.Card.GetComponent<CardData>();

                        if (cardData.GetAttackDamage() >= defendingCard.GetDefenseValue())
                        {
                            slotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defendingCard.gameObject);
                        }

                        break;
                    default:
                        Vector2Int defaultCurrentPosition = cardSlot.GetCardSlotPosition();
                        GameObject defaultAttackingPosition = gridManager.CardAt(defaultCurrentPosition.x, defaultCurrentPosition.y - 1);

                        if (defaultAttackingPosition == null)
                        {
                            Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                            continue;
                        }

                        CardSlot defaultSlotToAttack = defaultAttackingPosition.GetComponent<CardSlot>();

                        if (defaultSlotToAttack.Card == null) continue;
                        if (!defaultSlotToAttack.CardBelongsToPlayer()) continue;

                        CardData defaultDefendingCard = defaultSlotToAttack.Card.GetComponent<CardData>();
                        if (cardData.GetAttackDamage() >= defaultDefendingCard.GetDefenseValue())
                        {
                            defaultSlotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defaultDefendingCard.gameObject);
                        }

                        break;
                }
            }
        }

        OnAttackFinished?.Invoke(this, EventArgs.Empty);
    }
}
