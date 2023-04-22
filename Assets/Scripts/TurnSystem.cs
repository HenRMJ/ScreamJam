using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;
    public bool IsPlayersTurn { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("You have another turn system in your scene");
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        IsPlayersTurn = true;
    }
}
