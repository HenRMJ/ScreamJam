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

    [Header("Cursor States")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D selectCursor;
    [SerializeField] private Texture2D waitCursor;

    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private string spriteState;
    private State state;
    private Hand hand;
    private Texture2D currentCursor;

    private void Start()
    {
        foreach (MouseState mouseState in states)
        {
            sprites.Add(mouseState.state, mouseState.stateUI);
        }

        state = State.wait;
        currentCursor = waitCursor;

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
        Vector3 cursorSize = new Vector3(32, -32);

        Vector3 scaledOffset = new Vector3(UnityEngine.Screen.width * xOffset, UnityEngine.Screen.height * yOffset);
        UI.rectTransform.position = Input.mousePosition + scaledOffset + cursorSize;

        switch (state)
        {
            case State.wait:
                spriteState = "wait";
                break;
            case State.draw:
                Transform cursorTransform = Utils.GetTransformUnderCursor();

                spriteState = "draw";
                currentCursor = defaultCursor;

                UI.gameObject.SetActive(false);

                if (cursorTransform != null &&
                    cursorTransform.tag == "card" &&
                    cursorTransform.GetComponent<CardData>().InDeck)
                {
                    UI.gameObject.SetActive(true);
                    currentCursor = selectCursor;
                }

                break;
        }

        ToggleCursorUI();

        UI.sprite = sprites[spriteState];

        Cursor.SetCursor(currentCursor, Vector2.zero, CursorMode.Auto);
    }

    private void ToggleCursorUI()
    {
        if (state == State.draw) return;

        GameObject UIgameObject = UI.gameObject;
        UIgameObject.SetActive(true);

        if (state == State.wait) return;

        Transform cursorTransform = Utils.GetTransformUnderCursor();

        if (cursorTransform == null)
        {
            UIgameObject.SetActive(false);
            currentCursor = defaultCursor;
            return;
        }

        if (cursorTransform.tag != "cardSlot" &&
            cursorTransform.tag != "card" &&
            cursorTransform.tag != "bell")
        {
            UIgameObject.SetActive(false);
            currentCursor = defaultCursor;
            return;
        }

        if (cursorTransform.tag == "cardSlot")
        {
            CardSlot cardSlot = cursorTransform.GetComponent<CardSlot>();

            if (cardSlot.Card == null)
            {
                UIgameObject.SetActive(false);
                currentCursor = defaultCursor;
                return;
            }

            if (!cardSlot.CardBelongsToPlayer())
            {
                UIgameObject.SetActive(false);
                currentCursor = defaultCursor;
                return;
            }

            if (!Player.Instance.CanSummon &&
                hand.GetCardIsSelected() &&
                hand.GetSelectedCard().GetComponent<CardData>().Type == CardType.Monster)
            {
                UIgameObject.SetActive(false);
                currentCursor = defaultCursor;
                return;
            }
        }

        if (cursorTransform.tag == "bell")
        {
            spriteState = "bell";
            currentCursor = selectCursor;
            return;
        }

        if (cursorTransform.tag == "card" &&
            cursorTransform.GetComponent<CardData>().InDeck)
        {
            spriteState = "cannotDraw";
            currentCursor = defaultCursor;
        }
    }

    private void WaitingState_OnPlayerStartWaiting(object sender, EventArgs e)
    {
        state = State.wait;
        currentCursor = waitCursor;
    }

    private void DrawState_OnEnterDrawState(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayersTurn) return;
        state = State.draw;
        currentCursor = defaultCursor;
    }

    private void DecisionState_OnEnterDecisionState(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayersTurn) return;
        state = State.decision;
        currentCursor = defaultCursor;
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
                currentCursor = selectCursor;
            }

            if (cardData.InPlay &&
                hand.GetCardIsSelected())
            {
                spriteState = "clickToSacrifice";
                currentCursor = selectCursor;

                if (!cardData.CanMove)
                {
                    spriteState = "clickToSacrificeOnly";
                    currentCursor = selectCursor;
                }
            }

            if (cardData.InHand &&
                !cardData.InDeck)
            {
                spriteState = "clickToSelect";
                currentCursor = selectCursor;
            }

            if (cardData.transform == hand.GetSelectedCard())
            {
                spriteState = "clickToUnselect";
                currentCursor = selectCursor;
            }

            if (cardData.InPlay &&
                !hand.GetCardIsSelected() &&
                !cardData.CanMove)
            {
                spriteState = "cannotMove";
                currentCursor = defaultCursor;
            }
        }
    }
}
