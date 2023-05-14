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
                        CardSlot slotToAttack = gridManager.CardAt(currentPosition.x, currentPosition.y + 1);

                        // Enemy gets attacked right here
                        if (slotToAttack == null)
                        {
                            Enemy.Instance.SetBlood(-(cardData.GetAttackDamage() * avatarMultipilier));
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

                        // first attack slot is empty skips to the new attack slot
                        if (slotToAttack.Card == null)
                        {
                            OnSlotAttacked?.Invoke(this, slotToAttack.transform);
                            CardSlot secondSlotToAttack = gridManager.CardAt(currentPosition.x, currentPosition.y + 2);
                            yield return new WaitForSeconds(.1f);

                            // Enemy gets attacked here
                            if (secondSlotToAttack == null)
                            {
                                Enemy.Instance.SetBlood(-(cardData.GetAttackDamage() * avatarMultipilier));
                                yield return new WaitForSeconds(1f);
                                continue;
                            }
                            
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
                                
                                AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                                Destroy(secondDefendingCard.gameObject);
                                secondSlotToAttack.Card = null;
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
                            
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defendingCard.gameObject);
                            slotToAttack.Card = null;
                            yield return new WaitForSeconds(.1f);
                        }

                        break;
                    default:
                        Vector2Int defaultCurrentPosition = cardSlot.GetCardSlotPosition();
                        CardSlot defaultSlotToAttack = gridManager.CardAt(defaultCurrentPosition.x, defaultCurrentPosition.y + 1);

                        if (defaultSlotToAttack == null)
                        {
                            Enemy.Instance.SetBlood(-(cardData.GetAttackDamage() * avatarMultipilier));
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

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
                            
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defaultDefendingCard.gameObject);
                            defaultSlotToAttack.Card = null;
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
                        CardSlot slotToAttack = gridManager.CardAt(currentPosition.x, currentPosition.y - 1);

                        // Attacking Player
                        if (slotToAttack == null)
                        {
                            Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

                        if (slotToAttack.Card == null)
                        {
                            OnSlotAttacked?.Invoke(this, slotToAttack.transform);
                            CardSlot secondSlotToAttack = gridManager.CardAt(currentPosition.x, currentPosition.y - 2);
                            yield return new WaitForSeconds(.1f);

                            // Attacking Player
                            if (secondSlotToAttack == null)
                            {
                                Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                                yield return new WaitForSeconds(1f);
                                continue;
                            }

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
                                
                                AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                                Destroy(secondDefendingCard.gameObject);
                                secondSlotToAttack.Card = null;
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
                      
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defendingCard.gameObject);
                            slotToAttack.Card = null;
                            yield return new WaitForSeconds(.1f);
                        }

                        break;
                    default:
                        Vector2Int defaultCurrentPosition = cardSlot.GetCardSlotPosition();
                        CardSlot defaultSlotToAttack = gridManager.CardAt(defaultCurrentPosition.x, defaultCurrentPosition.y - 1);

                        // Attacking Player
                        if (defaultSlotToAttack == null)
                        {
                            Player.Instance.DealDamage(cardData.GetAttackDamage() * avatarMultipilier);
                            yield return new WaitForSeconds(1f);
                            continue;
                        }

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
                            
                            AkSoundEngine.PostEvent("MonsterAttack", gameObject);
                            Destroy(defaultDefendingCard.gameObject);
                            defaultSlotToAttack.Card = null;
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
