using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<Transform> deckList;

    [Header("Visual Deck Attributes")]
    [Tooltip("How much space is there between each card. If set to zero all cards will be in the same position")] [Range(0f, .01f)]
    [SerializeField] private float stackOffset;

    [Tooltip("Random rotational value to make the cards look like they're not stack perfectly squared")]
    [SerializeField] private float rotationalOffset;

    [SerializeField] private bool belongsToPlayer;

    private List<Transform> instantiatedCardList = new List<Transform>();

    private void Awake()
    {
        ShuffleCards();
        CreateCards();
    }

    private void Start()
    {
        Destroy(transform.GetChild(0).gameObject);
    }

    private void CreateCards()
    {
        Vector3 instatiationPosition = new Vector3(0, 0, 0);
        Vector3 offset = new Vector3(0, stackOffset, 0);

        for (int i = 0; i < deckList.Count; i++)
        {
            Transform deckCard = Instantiate(deckList[i], i == 0 ? transform.position : instatiationPosition + offset, Quaternion.Euler(-90f, Random.Range(-rotationalOffset, rotationalOffset), 0), transform);

            CardData cardData = deckCard.GetComponent<CardData>();
            cardData.SetBelongsToPlayer(belongsToPlayer);

            if (!belongsToPlayer)
            {
                Transform flipCard = cardData.transform.GetChild(0).transform;

                flipCard.localRotation = Quaternion.Euler(90, 180, -90);
            }

            instatiationPosition = deckCard.position;

            instantiatedCardList.Add(deckCard);
        }
    }

    private void ShuffleCards()
    {
        for (int i = 0; i < deckList.Count; i++)
        {
            Transform card = deckList[i];
            int randomPosition = Random.Range(0, i);
            deckList[i] = deckList[randomPosition];
            deckList[randomPosition] = card;
        }
    }

    public Transform DealCard(Hand hand)
    {
        if (instantiatedCardList.Count == 0)
        {
            Debug.Log("No more cards to deal");
            return null;
        }

        Transform cardToDeal = instantiatedCardList[0];
        CardData cardData = cardToDeal.GetComponent<CardData>();

        if (hand.BelongsToPlayer && !cardData.BelongsToPlayer()) return null;
        if (!hand.BelongsToPlayer && cardData.BelongsToPlayer()) return null;

        cardToDeal.parent = null;
        cardData.InHand = true;
        cardData.InDeck = false;

        instantiatedCardList.RemoveAt(0);
        AkSoundEngine.PostEvent("TakeCard", gameObject);
        return cardToDeal;
    }

    public int GetNumberOfCardsInDeck() => instantiatedCardList.Count;
}
