using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotAnimations : MonoBehaviour
{
    [Header("Static Settings")]
    [SerializeField] private Material borderMaterial;
    [SerializeField] private CardSlot cardSlot;

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed;
    [SerializeField] private Color onColor, offColor, attackColor;

    private Hand playerHand;
    private Color currentColor;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (Hand hand in FindObjectsOfType<Hand>())
        {
            if (hand.BelongsToPlayer)
            {
                playerHand = hand;
            }
        }

        CardSlot.OnCardVisualUpdate += CardSlot_OnCardVisualUpdate;
    }

    private void Update()
    {
        borderMaterial.color = new Color(Mathf.Lerp(borderMaterial.color.r, currentColor.r, Time.deltaTime * animationSpeed),
            Mathf.Lerp(borderMaterial.color.g, currentColor.g, Time.deltaTime * animationSpeed),
            Mathf.Lerp(borderMaterial.color.b, currentColor.b, Time.deltaTime * animationSpeed),
            Mathf.Lerp(borderMaterial.color.a, currentColor.a, Time.deltaTime * animationSpeed));
    }

    private void OnDisable()
    {
        CardSlot.OnCardVisualUpdate -= CardSlot_OnCardVisualUpdate;
    }

    private void CardSlot_OnCardVisualUpdate(object sender, bool e)
    {
        if (e)
        {
            currentColor = onColor;
        } else
        {
            currentColor = offColor;
        }
    }
}
