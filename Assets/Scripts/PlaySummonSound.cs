using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySummonSound : MonoBehaviour
{
    [SerializeField] private SummonSound summondSound;

    private Hand ownerHand;
    private CardData cardData;

    // Start is called before the first frame update
    void Start()
    {
        cardData = GetComponent<CardData>();

        if (cardData.BelongsToPlayer())
        {
            foreach (Hand hand in FindObjectsOfType<Hand>())
            {
                if (hand.BelongsToPlayer && cardData.BelongsToPlayer())
                {
                    ownerHand = hand;
                }
            }

            ownerHand.OnCardSummoned += OwnerHand_OnCardSummoned;
        }
        else
        {
            Enemy.Instance.OnEnemySummonedCard += Enemy_OnEnemySummonedCard;
        }
        

        
    }

    private void Enemy_OnEnemySummonedCard(object sender, Transform e)
    {
        if (e == transform)
        {
            switch (summondSound)
            {
                case SummonSound.High:
                    AkSoundEngine.PostEvent("Sum_MedMon", gameObject);
                    break;
                case SummonSound.Medium:
                    AkSoundEngine.PostEvent("Sum_MedMon", gameObject);
                    break;
                case SummonSound.Low:
                    AkSoundEngine.PostEvent("Sum_MedMon", gameObject);
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (ownerHand != null)
        {
            ownerHand.OnCardSummoned -= OwnerHand_OnCardSummoned;
        } else
        {
            Enemy.Instance.OnEnemySummonedCard -= Enemy_OnEnemySummonedCard;
        }
        
    }

    private void OwnerHand_OnCardSummoned(object sender, Transform e)
    {
        if (e == transform)
        {
            switch (summondSound)
            {
                case SummonSound.High:
                    AkSoundEngine.PostEvent("Sum_MedMon", gameObject);
                    break;
                case SummonSound.Medium:
                    AkSoundEngine.PostEvent("Sum_MedMon", gameObject);
                    break;
                case SummonSound.Low:
                    AkSoundEngine.PostEvent("Sum_MedMon", gameObject);
                    break;
            }
        }
    }

    public SummonSound GetSummonSound() => summondSound;
}
