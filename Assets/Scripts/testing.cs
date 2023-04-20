using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing : MonoBehaviour
{
    private GridSystem gridSystem;
    [SerializeField] private Transform visual;

    // Start is called before the first frame update
    void Start()
    {
        gridSystem = new GridSystem(4, 4, .13f);

        Debug.Log(new GridPosition(5, 7));
    }

    private void Update()
    {
        visual.position = gridSystem.GetWorldPosition(gridSystem.GetGridPosition(Utils.GetMousePosition()).x, gridSystem.GetGridPosition(Utils.GetMousePosition()).z);
    }
}
