using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class MouseActionUI : MonoBehaviour
{
    [Serializable]
    private class MouseState
    {
        [SerializeField] public Sprite stateUI;
        [SerializeField] public string state;
    }

    private enum State
    {
        draw,
        wait,
        decision
    }

    [SerializeField] private Image UI;
    [Range(-.1f, .1f)]
    [SerializeField] private float xOffset, yOffset;
    [SerializeField] private List<MouseState> states;
    
    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private string spriteState;
    private State state;
    private Hand hand;

    private void Start()
    {
        foreach (MouseState mouseState in states)
        {
            sprites.Add(mouseState.state, mouseState.stateUI);
        }

        state = State.wait;

        CardData.OnAnyCardHover += CardData_OnAnyCardHover;
        DecisionState.OnEnterDecisionState += DecisionState_OnEnterDecisionState;
        DrawState.OnEnterDrawState += DrawState_OnEnterDrawState;
        WaitingState.OnPlayerStartWaiting += WaitingState_OnPlayerStartWaiting;
    }

    private void OnDestroy()
    {
        CardData.OnAnyCardHover -= CardData_OnAnyCardHover;
        DecisionState.OnEnterDecisionState -= DecisionState_OnEnterDecisionState;
        DrawState.OnEnterDrawState -= DrawState_OnEnterDrawState;
        WaitingState.OnPlayerStartWaiting -= WaitingState_OnPlayerStartWaiting;
    }

    private void LateUpdate()
    {
        Vector3 scaledOffset = new Vector3(UnityEngine.Screen.width * xOffset, UnityEngine.Screen.height * yOffset);
        UI.rectTransform.position = Input.mousePosition + scaledOffset;

        switch (state)
        {
            case State.wait:
                spriteState = "wait";
                break;
            case State.draw:
                spriteState = "draw";
                break;
        }

        ToggleCursorUI();

        UI.sprite = sprites[spriteState];
    }

    private void ToggleCursorUI()
    {
        GameObject UIgameObject = UI.gameObject;
        UIgameObject.SetActive(true);

        if (state == State.wait) return;
        if (state == State.draw) return;

        Transform cursorTransform = Utils.GetTransformUnderCursor();

        if (cursorTransform == null)
        {
            UIgameObject.SetActive(false);
            return;
        }

        if (cursorTransform.tag != "cardSlot" &&
            cursorTransform.tag != "card" &&
            cursorTransform.tag != "bell")
        {
            UIgameObject.SetActive(false);
            return;
        }

        if (cursorTransform.tag == "cardSlot")
        {
            CardSlot cardSlot = cursorTransform.GetComponent<CardSlot>();

            if (cardSlot.Card == null)
            {
                UIgameObject.SetActive(false);
                return;
            }

            if (!cardSlot.CardBelongsToPlayer())
            {
                UIgameObject.SetActive(false);
                return;
            }

            if (!Player.Instance.CanSummon &&
                hand.GetCardIsSelected() &&
                hand.GetSelectedCard().GetComponent<CardData>().Type == CardType.Monster)
            {
                UIgameObject.SetActive(false);
                return;
            }
        }

        if (cursorTransform.tag == "bell")
        {
            spriteState = "bell";
            return;
        }

        if (cursorTransform.tag == "card" &&
            cursorTransform.GetComponent<CardData>().InDeck)
        {
            spriteState = "cannotDraw";
        }
    }

    private void WaitingState_OnPlayerStartWaiting(object sender, EventArgs e)
    {
        state = State.wait;
    }

    private void DrawState_OnEnterDrawState(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayersTurn) return;
        state = State.draw;
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayersTurn) return;
        state = State.decision;
    }

    private void CardData_OnAnyCardHover(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayersTurn) return;
        if (state == State.wait) return;
        if (state == State.draw) return;

        CardData cardData = (CardData)sender;

        if (cardData.BelongsToPlayer())
        {
            if (hand == null)
            {
                hand = cardData.GetHand();
            }

            if (cardData.InPlay && 
                cardData.CanMove && 
                !hand.GetCardIsSelected())
            {
                spriteState = "clickToMove";
            }

            if (cardData.InPlay &&
                hand.GetCardIsSelected())
            {
                spriteState = "clickToSacrifice";

                if (!cardData.CanMove)
                {
                    spriteState = "clickToSacrificeOnly";
                }
            }

            if (cardData.InHand &&
                !cardData.InDeck)
            {
                spriteState = "clickToSelect";
            }

            if (cardData.transform == hand.GetSelectedCard())
            {
                spriteState = "clickToUnselect";
            }

            if (cardData.InPlay &&
                !hand.GetCardIsSelected() &&
                !cardData.CanMove)
            {
                spriteState = "cannotMove";
            }
        }
    }
}
