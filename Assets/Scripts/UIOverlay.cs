using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIOverlay : MonoBehaviour
{
    [Header("TMP Card Stat Fields")]
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI attackField;
    [SerializeField] private TextMeshProUGUI bloodCostField;
    [SerializeField] private TextMeshProUGUI defenseField;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject descriptionUI;


    // Start is called before the first frame update
    private void Start()
    {
        CardData.OnAnyCardHover += CardData_OnAnyCardHover;
        DecisionState.OnEnterDecisionState += DecisionState_OnEnterDecisionState;
        DrawState.OnEnterDrawState += DrawState_OnEnterDrawState;
        WaitingState.OnPlayerStartWaiting += WaitingState_OnPlayerStartWaiting;
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
        WaitingState.OnPlayerStartWaiting -= WaitingState_OnPlayerStartWaiting;
    }

    private void DrawState_OnEnterDrawState(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayersTurn) return;
        animator.SetTrigger("drawState");
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayersTurn) return;

        if (TurnSystem.Instance.AttackedThisRound)
        {
            animator.SetTrigger("postState");
        }
        else
        {
            animator.SetTrigger("mainState");
        }
    }

    private void WaitingState_OnPlayerStartWaiting(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayersTurn)
        {
            animator.SetTrigger("attackState");
        } else
        {
            animator.SetTrigger("enemyState");
        }
    }

    private void CardData_OnAnyCardHover(object sender, EventArgs e)
    {
        CardData cardData = (CardData)sender;

        nameField.text = cardData.GetCardName();
        descriptionField.text = cardData.GetCardUIDescription();
        attackField.text = cardData.GetAttackDamage().ToString();
        defenseField.text = cardData.GetDefenseValue().ToString();
        bloodCostField.text = cardData.GetBloodCost().ToString();
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
