using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour
{
    public static Bell Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("You have another bell in your scene");
            return;
        }
        Instance = this;
    }

    public bool CheckIfClickBell()
    {
        if (!TurnSystem.Instance.IsPlayersTurn) return false;
        if (Utils.GetTransformUnderCursor() != transform) return false;
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }
}
