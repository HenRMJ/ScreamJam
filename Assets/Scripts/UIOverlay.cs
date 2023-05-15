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
    [SerializeField] private TextMeshProUGUI largeDisplayField;

    [Header("Other Fields")]
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
        if (Player.Instance.GetPlayerHand().GetDeck().GetNumberOfCardsInDeck() <= 0) return;
        largeDisplayField.text = "Draw a Card";
        animator.SetTrigger("largeDisplay");
        animator.SetTrigger("drawState");
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayersTurn) return;

        if (TurnSystem.Instance.AttackedThisRound)
        {
            largeDisplayField.text = "Ring Bell to End Turn";
            animator.SetTrigger("postState");
            animator.SetTrigger("largeDisplay");
        }
        else
        {
            largeDisplayField.text = "Ring Bell to Attack";
            animator.SetTrigger("mainState");
            animator.SetTrigger("largeDisplay");
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
        attackField.transform.parent.gameObject.SetActive(true);
        defenseField.transform.parent.gameObject.SetActive(true);

        CardData cardData = (CardData)sender;

        if (cardData.Type == CardType.Spell)
        {
            attackField.transform.parent.gameObject.SetActive(false);
            defenseField.transform.parent.gameObject.SetActive(false);
        }

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

        if (!cardData.BelongsToPlayer() && !cardData.InPlay)
        {
            descriptionUI.SetActive(false);
            return;
        }

        descriptionUI.SetActive(true);
    }
}
