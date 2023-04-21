using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public static List<CardSlot> CardSlots { get; private set; }

    private void Start()
    {
        CardSlots = new List<CardSlot>();

        foreach (CardSlot cardSlot in FindObjectsOfType<CardSlot>())
        {
            CardSlots.Add(cardSlot);
        }
    }
}
