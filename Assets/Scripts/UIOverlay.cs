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
    [SerializeField] private TextMeshProUGUI cardToolTip;

    [Header("Other Fields")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject descriptionUI;

    // Start is called before the first frame update
    private void Start()
    {
        CardData.OnAnyCardHover += CardData_OnAnyCardHover;

        DrawPhase.OnEnterDrawState += DrawState_OnEnterDrawState;
        MainPhase.OnEnterDecisionState += DecisionState_OnEnterDecisionState;
        AttackPhase.OnAttackStateStarted += AttackPhase_OnAttackStarted;
        PostPhase.OnEnterDecisionState += PostPhase_OnEnterDecisionState;
        EnemyPhase.OnEnterEnemyState += WaitingState_OnPlayerStartWaiting;

        Player.Instance.GetPlayerHand().OnCardNotSummoned += PlayerHand_OnCardNotSummoned;
    }

    private void Update()
    {
        UpdateCardUI();
    }

    private void OnDisable()
    {
        CardData.OnAnyCardHover -= CardData_OnAnyCardHover;

        DrawPhase.OnEnterDrawState -= DrawState_OnEnterDrawState;
        MainPhase.OnEnterDecisionState -= DecisionState_OnEnterDecisionState;
        AttackPhase.OnAttackStateStarted -= AttackPhase_OnAttackStarted;
        PostPhase.OnEnterDecisionState -= PostPhase_OnEnterDecisionState;
        EnemyPhase.OnEnterEnemyState -= WaitingState_OnPlayerStartWaiting;
    }

    private void DrawState_OnEnterDrawState(object sender, EventArgs e)
    {
        if (Player.Instance.GetPlayerHand().GetDeck().GetNumberOfCardsInDeck() <= 0) return;

        largeDisplayField.text = "Draw a Card";
        animator.SetTrigger("largeDisplay");
        animator.SetTrigger("drawState");
    }

    private void PostPhase_OnEnterDecisionState(object sender, EventArgs e)
    {
        largeDisplayField.text = "Ring Bell to End Turn";
        animator.SetTrigger("postState");
        animator.SetTrigger("largeDisplay");
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        largeDisplayField.text = "Ring Bell to Attack";
        animator.SetTrigger("mainState");
        animator.SetTrigger("largeDisplay");
    }

    private void WaitingState_OnPlayerStartWaiting(object sender, EventArgs e)
    {
        animator.SetTrigger("enemyState");
    }

    private void AttackPhase_OnAttackStarted(object sender, EventArgs e)
    {
        animator.SetTrigger("attackState");
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

    private void PlayerHand_OnCardNotSummoned(object sender, string e)
    {
        cardToolTip.text = e;
        animator.SetTrigger("tooltip");
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
