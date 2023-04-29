using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public static PlayArea Instance;

    public event EventHandler OnAttackFinished;
    public event EventHandler<Transform> OnSlotAttacked;

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
        StartCoroutine(AllCardsAttack(TurnSystem.Instance.IsPlayersTurn));
    }

    public void EnemyAttacks()
    {
        StartCoroutine(AllCardsAttack(false));
    }

    public IEnumerator AllCardsAttack(bool playerAttacking)
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

                        // Enemy gets attacked right here
                        if (attackingPosition == null)
                        {
                            Enemy.Instance.Blood -= cardData.GetAttackDamage() * avatarMultipilier;
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

                        CardSlot slotToAttack = attackingPosition.GetComponent<CardSlot>();

                        // first attack slot is empty skips to the new attack slot
                        if (slotToAttack.Card == null)
                        {
                            OnSlotAttacked?.Invoke(this, slotToAttack.transform);
                            GameObject attackingSecondPosition = gridManager.CardAt(currentPosition.x, currentPosition.y + 2);
                            yield return new WaitForSeconds(.1f);

                            // Enemy gets attacked here
                            if (attackingSecondPosition == null)
                            {
                                Enemy.Instance.Blood -= cardData.GetAttackDamage() * avatarMultipilier;
                                yield return new WaitForSeconds(1f);
                                continue;
                            }

                            CardSlot secondSlotToAttack = attackingSecondPosition.GetComponent<CardSlot>();
                            
                            // Second Card slot was empty
                            if (secondSlotToAttack.Card == null)
                            {
                                OnSlotAttacked?.Invoke(this, secondSlotToAttack.transform);
                                yield return new WaitForSeconds(1f);
                                continue;
                            }
                            if (secondSlotToAttack.CardBelongsToPlayer()) continue;

                            CardData secondDefendingCard = secondSlotToAttack.Card.GetComponent<CardData>();

                            if (cardData.GetAttackDamage() >= secondDefendingCard.GetDefenseValue())
                            {
                                OnSlotAttacked?.Invoke(this, secondSlotToAttack.transform);
                                secondSlotToAttack.Card = null;
                                AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                                Destroy(secondDefendingCard.gameObject);
                                yield return new WaitForSeconds(.1f);
                            }
                        }

                        // there was no card in slot
                        if (slotToAttack.Card == null)
                        {
                            yield return new WaitForSeconds(1f);
                            continue;
                        }
                        if (slotToAttack.CardBelongsToPlayer()) continue;

                        CardData defendingCard = slotToAttack.Card.GetComponent<CardData>();

                        if (cardData.GetAttackDamage() >= defendingCard.GetDefenseValue())
                        {
                            OnSlotAttacked?.Invoke(this, slotToAttack.transform);
                            slotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defendingCard.gameObject);
                            yield return new WaitForSeconds(.1f);
                        }

                        break;
                    default:
                        Vector2Int defaultCurrentPosition = cardSlot.GetCardSlotPosition();
                        GameObject defaultAttackingPosition = gridManager.CardAt(defaultCurrentPosition.x, defaultCurrentPosition.y + 1);

                        if (defaultAttackingPosition == null)
                        {
                            Enemy.Instance.Blood -= cardData.GetAttackDamage() * avatarMultipilier;
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

                        CardSlot defaultSlotToAttack = defaultAttackingPosition.GetComponent<CardSlot>();

                        // Slot was empty
                        if (defaultSlotToAttack.Card == null)
                        {
                            OnSlotAttacked?.Invoke(this, defaultSlotToAttack.transform);
                            yield return new WaitForSeconds(1f);
                            continue;
                        }
                        if (defaultSlotToAttack.CardBelongsToPlayer()) continue;

                        CardData defaultDefendingCard = defaultSlotToAttack.Card.GetComponent<CardData>();

                        if (cardData.GetAttackDamage() >= defaultDefendingCard.GetDefenseValue())
                        {
                            OnSlotAttacked?.Invoke(this, defaultSlotToAttack.transform);
                            defaultSlotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defaultDefendingCard.gameObject);
                            yield return new WaitForSeconds(.1f);
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

                        // Attacking Player
                        if (attackingPosition == null)
                        {
                            Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

                        CardSlot slotToAttack = attackingPosition.GetComponent<CardSlot>();

                        if (slotToAttack.Card == null)
                        {
                            OnSlotAttacked?.Invoke(this, slotToAttack.transform);
                            GameObject attackingSecondPosition = gridManager.CardAt(currentPosition.x, currentPosition.y - 2);
                            yield return new WaitForSeconds(.1f);

                            // Attacking Player
                            if (attackingSecondPosition == null)
                            {
                                Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                                yield return new WaitForSeconds(1f);
                                continue;
                            }

                            CardSlot secondSlotToAttack = attackingSecondPosition.GetComponent<CardSlot>();

                            if (secondSlotToAttack.Card == null)
                            {
                                OnSlotAttacked?.Invoke(this, secondSlotToAttack.transform);
                                yield return new WaitForSeconds(1f);
                                continue;
                            }
                            if (!secondSlotToAttack.CardBelongsToPlayer()) continue;

                            CardData secondDefendingCard = secondSlotToAttack.Card.GetComponent<CardData>();

                            if (cardData.GetAttackDamage() >= secondDefendingCard.GetDefenseValue())
                            {
                                OnSlotAttacked?.Invoke(this, secondSlotToAttack.transform);
                                secondSlotToAttack.Card = null;
                                AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                                Destroy(secondDefendingCard.gameObject);
                                yield return new WaitForSeconds(.1f);
                            }
                        }

                        if (slotToAttack.Card == null)
                        {
                            yield return new WaitForSeconds(1f);
                            continue;
                        }
                        if (!slotToAttack.CardBelongsToPlayer()) continue;

                        CardData defendingCard = slotToAttack.Card.GetComponent<CardData>();

                        if (cardData.GetAttackDamage() >= defendingCard.GetDefenseValue())
                        {
                            OnSlotAttacked?.Invoke(this, slotToAttack.transform);
                            slotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defendingCard.gameObject);
                            yield return new WaitForSeconds(.1f);
                        }

                        break;
                    default:
                        Vector2Int defaultCurrentPosition = cardSlot.GetCardSlotPosition();
                        GameObject defaultAttackingPosition = gridManager.CardAt(defaultCurrentPosition.x, defaultCurrentPosition.y - 1);

                        // Attacking Player
                        if (defaultAttackingPosition == null)
                        {
                            Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

                        CardSlot defaultSlotToAttack = defaultAttackingPosition.GetComponent<CardSlot>();

                        if (defaultSlotToAttack.Card == null) 
                        {
                            OnSlotAttacked?.Invoke(this, defaultSlotToAttack.transform);
                            yield return new WaitForSeconds(1f);
                            continue; 
                        }
                        if (!defaultSlotToAttack.CardBelongsToPlayer()) continue;

                        CardData defaultDefendingCard = defaultSlotToAttack.Card.GetComponent<CardData>();
                        if (cardData.GetAttackDamage() >= defaultDefendingCard.GetDefenseValue())
                        {
                            OnSlotAttacked?.Invoke(this, defaultSlotToAttack.transform);
                            defaultSlotToAttack.Card = null;
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defaultDefendingCard.gameObject);
                            yield return new WaitForSeconds(.1f);
                        }

                        break;
                }
            }
        }

        yield return new WaitForSeconds(.1f);
        Debug.Log("Attack Finished");
        OnAttackFinished?.Invoke(this, EventArgs.Empty);
    }
}
