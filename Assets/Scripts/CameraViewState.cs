using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class CameraViewState : MonoBehaviour
{
    private enum State
    {
        Top,
        Board,
        Hand
    }

    [SerializeField] CinemachineVirtualCamera[] virtualCameras;

    private PlayerControlAsset controller;
    private State currentState;

    // Start is called before the first frame update
    private void Awake()
    {
        controller = new PlayerControlAsset();
        controller.Enable();
    }

    private void Start()
    {
        controller.Table.CameraSwitch.started += OnCameraSwitchStarted;
        ResetVirtualCameras();
        virtualCameras[1].Priority = 5;
        currentState = State.Board;
    }

    private void OnCameraSwitchStarted(InputAction.CallbackContext obj)
    {
        float direction = obj.ReadValue<float>();

        ResetVirtualCameras();
        SwitchCameras(direction);
    }

    private void SwitchCameras(float direction)
    {
        switch (currentState)
        {
            case State.Board:
                if (direction == 1f)
                {
                    virtualCameras[0].Priority = 5;
                    currentState = State.Top;
                }
                else
                {
                    virtualCameras[2].Priority = 5;
                    currentState = State.Hand;
                }
                break;
            case State.Hand:
                if (direction == 1f)
                {
                    virtualCameras[1].Priority = 5;
                    currentState = State.Board;
                }
                break;
            case State.Top:
                if (direction == -1f)
                {
                    virtualCameras[1].Priority = 5;
                    currentState = State.Board;
                }
                break;
        }
    }

    private void ResetVirtualCameras()
    {
        foreach (CinemachineVirtualCamera vcam in virtualCameras)
        {
            vcam.Priority = 1;
        }
    }
}
