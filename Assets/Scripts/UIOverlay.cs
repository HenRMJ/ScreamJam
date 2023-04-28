using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardDescription, overlay, smallOverlay;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject descriptionUI;

    [SerializeField] private string draw, decision, attack;
    [SerializeField] [TextArea(3,8)]
    private string drawS, decisionS, attackS;

    private bool overlayOn;

    // Start is called before the first frame update
    private void Start()
    {
        CardData.OnAnyCardHover += CardData_OnAnyCardHover;
        DecisionState.OnEnterDecisionState += DecisionState_OnEnterDecisionState;
        DrawState.OnEnterDrawState += DrawState_OnEnterDrawState;

        smallOverlay.text = string.Empty;
        overlayOn = true;
        smallOverlay.gameObject.SetActive(overlayOn);
    }

    private void Update()
    {
        UpdateCardUI();
    }

    private void OnDisable()
    {
        CardData.OnAnyCardHover -= CardData_OnAnyCardHover;
        DecisionState.OnEnterDecisionState -= DecisionState_OnEnterDecisionState;
        DrawState.OnEnterDrawState -= DrawState_OnEnterDrawState;
    }

    private void DrawState_OnEnterDrawState(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayersTurn) return;
        overlay.text = draw;
        smallOverlay.text = drawS;
        animator.SetTrigger("newState");
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayersTurn) return;

        if (TurnSystem.Instance.AttackedThisRound)
        {
            overlay.text = attack;
            smallOverlay.text = attackS;
        }
        else
        {
            overlay.text = decision;
            smallOverlay.text = decisionS;
        }

        animator.SetTrigger("newState");
    }

    private void CardData_OnAnyCardHover(object sender, EventArgs e)
    {
        CardData cardData = (CardData)sender;
        cardDescription.text = cardData.GetCardUIDescription();
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

    public void ToggleText()
    {
        overlayOn = !overlayOn;
        smallOverlay.gameObject.SetActive(overlayOn);
    }
}
