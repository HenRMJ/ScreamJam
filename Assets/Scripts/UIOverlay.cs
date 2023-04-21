using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private GameObject descriptionUI;

    // Start is called before the first frame update
    void Start()
    {
        CardData.OnAnyCardHover += CardData_OnAnyCardHover;
    }

    private void CardData_OnAnyCardHover(object sender, EventArgs e)
    {
        CardData cardData = (CardData)sender;
        cardDescription.text = cardData.GetCardUIDescription();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        GameObject card = Utils.GetCardObjectUnderCursor();

        if (card == null)
        {
            descriptionUI.SetActive(false);
            return;
        }

        CardData cardData = card.GetComponent<CardData>();

        if (cardData.InDeck)
        {
            descriptionUI.SetActive(false);
            return;
        }

        descriptionUI.SetActive(true);
    }
}
