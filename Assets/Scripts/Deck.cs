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

    private void Start()
    {
        Destroy(transform.GetChild(0).gameObject);
        ShuffleCards();
        CreateCards();
    }

    private void CreateCards()
    {
        Vector3 instatiationPosition = new Vector3(0, 0, 0);
        Vector3 offset = new Vector3(0, stackOffset, 0);

        for (int i = 0; i < deckList.Count; i++)
        {
            Transform cardPosition = Instantiate(deckList[i], i == 0 ? transform.position : instatiationPosition + offset, Quaternion.Euler(-90f, Random.Range(-rotationalOffset, rotationalOffset), 0), transform);
            instatiationPosition = cardPosition.position;
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
}