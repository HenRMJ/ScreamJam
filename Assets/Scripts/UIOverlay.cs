using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardDescription, overlay;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject descriptionUI;


    [SerializeField] private string draw, decision, attack;

    // Start is called before the first frame update
    void Start()
    {
        CardData.OnAnyCardHover += CardData_OnAnyCardHover;
        DecisionState.OnEnterDecisionState += DecisionState_OnEnterDecisionState;
        DrawState.OnEnterDrawState += DrawState_OnEnterDrawState;
    }

    private void DrawState_OnEnterDrawState(object sender, EventArgs e)
    {
        overlay.text = draw;
        animator.SetTrigger("newState");
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.AttackedThisRound)
        {
            overlay.text = attack;
        }
        else
        {
            overlay.text = decision;
        }

        animator.SetTrigger("newState");
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
