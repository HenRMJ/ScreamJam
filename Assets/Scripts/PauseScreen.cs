using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    private void Start()
    {
        canvas.SetActive(false);
    }

    public void Rules()
    {
        canvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        canvas.SetActive(false);
        Time.timeScale = 1;
    }
}
