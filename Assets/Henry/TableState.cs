using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableState : MonoBehaviour
{
    [SerializeField] private float topViewLimit;
    [SerializeField] private float mouseSpeed;

    PlayerControlAsset controller;

    // Start is called before the first frame update
    private void Awake()
    {
        controller = new PlayerControlAsset();
        controller.Table.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookValue = controller.Table.Look.ReadValue<Vector2>();
        transform.position = new Vector3(transform.position.x + lookValue.x * mouseSpeed * Time.deltaTime, .5f, transform.position.z + lookValue.y * mouseSpeed * Time.deltaTime);
    }
}
